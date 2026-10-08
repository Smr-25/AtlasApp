using System.Text;
using Atlas.Application;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.Writer;

namespace Atlas.Infrastructure.Tools;

public sealed class PdfTools
{
    public static int[] Pages(string value, int count)
    {
        var pages = new List<int>();
        foreach (var part in value.Split(','))
        {
            var range = part.Trim().Split('-');
            if (range.Length is < 1 or > 2 || !int.TryParse(range[0], out var first) || first < 1 || first > count)
                throw new ArgumentException($"Pages must be between 1 and {count}.");
            var last = first;
            if (range.Length == 2 && (!int.TryParse(range[1], out last) || last < first || last > count))
                throw new ArgumentException("Use ascending ranges, such as 1-4.");
            if (last - first + 1 > 10000 - pages.Count) throw new ArgumentException("Select at most 10,000 pages.");
            pages.AddRange(Enumerable.Range(first, last - first + 1));
        }
        return pages.ToArray();
    }

    public async Task<ToolResult> Run(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        var paths = (r.Paths ?? []).Select(Path.GetFullPath).ToArray();
        if (paths.Length == 0) throw new ArgumentException("Choose at least one file.");
        if (!new[] { "pdf.merge", "pdf.split", "pdf.pages", "pdf.images", "pdf.text" }.Contains(r.Tool)) throw new ArgumentException("Unknown PDF operation.");
        var folder = r.Tool == "pdf.text" ? "" : FileTools.OutputFolder(r);
        var files = new List<string>();
        async Task Save(PdfDocumentBuilder builder, string name)
        {
            ct.ThrowIfCancellationRequested();
            var temporary = Path.Combine(folder, ".atlas-" + Guid.NewGuid() + ".pdf");
            try
            {
                await File.WriteAllBytesAsync(temporary, builder.Build(), ct);
                ct.ThrowIfCancellationRequested();
                var target = FileTools.UniquePath(folder, name);
                File.Move(temporary, target, false); files.Add(target);
            }
            finally { File.Delete(temporary); }
        }
        static PdfDocument Read(string path)
        {
            var document = PdfDocument.Open(path);
            if (document.IsEncrypted || document.NumberOfPages == 0 || document.NumberOfPages > 10000)
            { document.Dispose(); throw new ArgumentException("Use an unencrypted PDF containing 1–10,000 pages."); }
            return document;
        }
        if (r.Tool == "pdf.text")
        {
            var text = new StringBuilder();
            foreach (var path in paths)
            {
                using var document = Read(path);
                for (var i = 1; i <= document.NumberOfPages; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    text.AppendLine($"— {Path.GetFileName(path)} · Page {i} —");
                    var pageText = ContentOrderTextExtractor.GetText(document.GetPage(i));
                    text.AppendLine(string.IsNullOrWhiteSpace(pageText) ? "[No selectable text]" : pageText);
                    if (text.Length > 8_000_000) throw new ArgumentException("Extracted text exceeds 8 million characters. Select a smaller PDF.");
                    report(new("progress", $"Page {i}", (double)i / document.NumberOfPages));
                }
            }
            return new(text.ToString());
        }
        using var combined = new PdfDocumentBuilder();
        if (r.Tool == "pdf.images")
        {
            foreach (var path in paths)
            {
                ct.ThrowIfCancellationRequested();
                var info = await Image.IdentifyAsync(path, ct);
                if ((long)info.Width * info.Height > 80_000_000) throw new ArgumentException("Image exceeds 80 megapixels.");
                using var image = await Image.LoadAsync(path, ct);
                image.Mutate(x => x.AutoOrient());
                using var png = new MemoryStream(); await image.SaveAsPngAsync(png, ct);
                combined.AddPage(image.Width, image.Height).AddPng(png.ToArray(), new PdfRectangle(0, 0, image.Width, image.Height));
            }
            await Save(combined, "Atlas-images.pdf");
        }
        else
        {
            if (r.Tool != "pdf.merge" && paths.Length != 1) throw new ArgumentException("Choose one PDF.");
            var total = 0;
            foreach (var path in paths)
            {
                using var document = Read(path);
                var pages = r.Tool == "pdf.pages" ? Pages(r.Option("pages", "1"), document.NumberOfPages) : Enumerable.Range(1, document.NumberOfPages).ToArray();
                if ((total += pages.Length) > 10000) throw new ArgumentException("The result exceeds 10,000 pages.");
                foreach (var page in pages)
                {
                    ct.ThrowIfCancellationRequested();
                    if (r.Tool == "pdf.split")
                    {
                        using var single = new PdfDocumentBuilder(); single.AddPage(document, page);
                        await Save(single, $"Atlas-page-{page}.pdf");
                    }
                    else combined.AddPage(document, page);
                    report(new("progress", $"Page {page}", Files: files.ToArray()));
                }
            }
            if (r.Tool != "pdf.split") await Save(combined, r.Tool == "pdf.pages" ? "Atlas-selected.pdf" : "Atlas-combined.pdf");
        }
        return new($"Created {files.Count} files. Original documents were preserved.", files.ToArray());
    }
}
