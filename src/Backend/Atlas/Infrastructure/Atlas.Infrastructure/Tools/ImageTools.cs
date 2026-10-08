using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Atlas.Application;
using Atlas.Application.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Atlas.Infrastructure.Tools;

public class ImageTools : IImageTools
{
    private static IImageEncoder Encoder(string format, int quality) => format.ToLowerInvariant() switch
    {
        "png" => new PngEncoder(), "jpg" or "jpeg" => new JpegEncoder { Quality = quality },
        "webp" => new WebpEncoder { Quality = quality }, _ => throw new ArgumentException("Choose png, jpg or webp.")
    };

    public async Task<Stream> ConvertImageAsync(Stream inputStream, string targetFormat, CancellationToken cancellationToken)
    {
        using var image = await Image.LoadAsync(inputStream, cancellationToken);
        var output = new MemoryStream();
        await image.SaveAsync(output, Encoder(targetFormat, 80), cancellationToken);
        output.Position = 0;
        return output;
    }

    public async Task<ToolResult> Batch(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        var inputs = FileTools.Inputs(r);
        var folder = FileTools.OutputFolder(r);
        var format = r.Option("format", "jpg");
        var encoder = Encoder(format, r.Number("quality", 85, 1, 100));
        var width = r.Number("width", 0, 0, 16000);
        var height = r.Number("height", 0, 0, 16000);
        var outputs = new List<string>();
        var rows = new List<string[]> { new[] { "Input", "Result", "Before bytes", "After bytes" } };
        for (var i = 0; i < inputs.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            string? temp = null;
            try
            {
                var info = await Image.IdentifyAsync(inputs[i], ct);
                if ((long)info.Width * info.Height > 80000000) throw new ArgumentException("Image exceeds 80 megapixels.");
                using var image = await Image.LoadAsync(inputs[i], ct);
                image.Mutate(x => x.AutoOrient());
                if (width > 0 || height > 0)
                    image.Mutate(x => x.Resize(new ResizeOptions { Size = new Size(width, height), Mode = ResizeMode.Max, Sampler = KnownResamplers.Lanczos3 }));
                if (r.Flag("strip"))
                {
                    image.Metadata.ExifProfile = null; image.Metadata.IptcProfile = null;
                    image.Metadata.XmpProfile = null; image.Metadata.IccProfile = null;
                }
                var prefix = r.Option("prefix");
                if (prefix.IndexOfAny(['/', '\\', '\0', ':']) >= 0) throw new ArgumentException("Invalid filename prefix.");
                var stem = r.Flag("number") ? $"{i + 1:D3}" : Path.GetFileNameWithoutExtension(inputs[i]);
                var output = FileTools.UniquePath(folder, prefix + stem + "." + format);
                temp = Path.Combine(folder, ".atlas-" + Guid.NewGuid() + ".tmp");
                await image.SaveAsync(temp, encoder, ct);
                ct.ThrowIfCancellationRequested();
                File.Move(temp, output, false); temp = null;
                outputs.Add(output);
                rows.Add([inputs[i], output, new FileInfo(inputs[i]).Length.ToString(), new FileInfo(output).Length.ToString()]);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                rows.Add([inputs[i], "Error: " + ex.Message, "", ""]);
            }
            finally { if (temp != null) File.Delete(temp); }
            report(new("progress", Path.GetFileName(inputs[i]), (double)(i + 1) / inputs.Length, outputs.ToArray(), rows.ToArray()));
        }
        var imageCount = outputs.Count;
        var failed = inputs.Length - imageCount;
        if (imageCount == 0) throw new IOException("No images were exported.\n" + string.Join('\n', rows.Skip(1).Select(row => row[0] + ": " + row[1])));
        if (r.Flag("zip"))
        {
            var zip = await new FileTools().Archive(new("files.zip", Paths: outputs.ToArray(), OutputDirectory: folder), report, ct);
            outputs.AddRange(zip.Files ?? []);
        }
        return new($"Exported {imageCount} images; {failed} failed. Original images were preserved.", outputs.ToArray(), rows.ToArray());
    }

    public async Task<ToolResult> Metadata(ToolRequest r, CancellationToken ct)
    {
        var rows = new List<string[]> { new[] { "Property", "Value" } };
        foreach (var path in FileTools.Inputs(r))
        {
            var info = await Image.IdentifyAsync(path, ct);
            rows.Add(["File", path]); rows.Add(["Dimensions", $"{info.Width} × {info.Height}"]);
            rows.Add(["Format", info.Metadata.DecodedImageFormat?.Name ?? "Unknown"]);
            if (info.Metadata.ExifProfile is { } exif)
                foreach (var value in exif.Values) rows.Add([value.Tag.ToString(), value.ToString() ?? ""]);
            rows.Add(["ICC profile", info.Metadata.IccProfile != null ? "Present" : "Absent"]);
            rows.Add(["XMP profile", info.Metadata.XmpProfile != null ? "Present" : "Absent"]);
        }
        return new("Image metadata", Rows: rows.ToArray());
    }

    public string OptimizeSvg(string svgContent)
    {
        try
        {
            var doc = XDocument.Parse(svgContent);
            var root = doc.Root;
            if (root == null) return svgContent;

            RemoveComments(root);
            RemoveMetadata(root);

            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            doc.Save(writer, SaveOptions.DisableFormatting);
            return sb.ToString();
        }
        catch
        {
            return svgContent;
        }
    }

    public ContrastCheckResult CheckContrast(string foregroundHex, string backgroundHex)
    {
        var fgLuminance = GetRelativeLuminance(foregroundHex);
        var bgLuminance = GetRelativeLuminance(backgroundHex);

        var lighter = Math.Max(fgLuminance, bgLuminance);
        var darker = Math.Min(fgLuminance, bgLuminance);
        var ratio = (lighter + 0.05) / (darker + 0.05);

        var passesAA = ratio >= 4.5;
        var passesAAA = ratio >= 7.0;
        var level = passesAAA ? "AAA" : passesAA ? "AA" : "Fail";

        return new ContrastCheckResult(Math.Round(ratio, 2), passesAA, passesAAA, level);
    }

    public AspectRatioResult CalculateAspectRatio(int width, int height)
    {
        if (width <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive.");
        var gcd = Gcd(width, height);
        var w = width / gcd;
        var h = height / gcd;
        return new AspectRatioResult($"{w}:{h}", w, h);
    }

    public List<string> ExtractColorsFromImage(Stream imageStream, int count = 5)
    {
        var info = SixLabors.ImageSharp.Image.Identify(imageStream);
        if ((long)info.Width * info.Height > 80000000) throw new ArgumentException("Image exceeds 80 megapixels.");
        imageStream.Position = 0;
        using var image = SixLabors.ImageSharp.Image.Load<SixLabors.ImageSharp.PixelFormats.Rgba32>(imageStream);
        var counts = new Dictionary<string, int>();
        var step = Math.Max(1, (int)Math.Sqrt((long)image.Width * image.Height / 50000.0));
        for (var y = 0; y < image.Height; y += step)
        for (var x = 0; x < image.Width; x += step)
        {
            var pixel = image[x, y];
            if (pixel.A < 128) continue;
            var key = $"#{pixel.R / 16 * 16:X2}{pixel.G / 16 * 16:X2}{pixel.B / 16 * 16:X2}";
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }
        return counts.OrderByDescending(x => x.Value).Take(Math.Clamp(count, 1, 16)).Select(x => x.Key).ToList();
    }

    public string ExtractCssVariables(List<ColorVariable> colors, string format = "css")
    {
        var sb = new StringBuilder();

        if (format.Equals("tailwind", StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine("module.exports = {");
            sb.AppendLine("  theme: {");
            sb.AppendLine("    extend: {");
            sb.AppendLine("      colors: {");
            foreach (var color in colors)
                sb.AppendLine($"        '{Slugify(color.Name)}': '{color.HexCode}',");
            sb.AppendLine("      }");
            sb.AppendLine("    }");
            sb.AppendLine("  }");
            sb.AppendLine("}");
        }
        else
        {
            sb.AppendLine(":root {");
            foreach (var color in colors)
                sb.AppendLine($"  --{Slugify(color.Name)}: {color.HexCode};");
            sb.AppendLine("}");
        }

        return sb.ToString();
    }

    private static double GetRelativeLuminance(string hex)
    {
        hex = hex.Trim().TrimStart('#');
        if (!Regex.IsMatch(hex, "^(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")) throw new ArgumentException("Use a 3 or 6 digit hex color.");
        if (hex.Length == 3)
            hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";

        var r = int.Parse(hex[..2], System.Globalization.NumberStyles.HexNumber) / 255.0;
        var g = int.Parse(hex[2..4], System.Globalization.NumberStyles.HexNumber) / 255.0;
        var b = int.Parse(hex[4..6], System.Globalization.NumberStyles.HexNumber) / 255.0;

        r = r <= 0.03928 ? r / 12.92 : Math.Pow((r + 0.055) / 1.055, 2.4);
        g = g <= 0.03928 ? g / 12.92 : Math.Pow((g + 0.055) / 1.055, 2.4);
        b = b <= 0.03928 ? b / 12.92 : Math.Pow((b + 0.055) / 1.055, 2.4);

        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private static string Slugify(string name) =>
        Regex.Replace(name.ToLower().Trim(), @"[^a-z0-9]+", "-").Trim('-');

    private static void RemoveComments(XElement element)
    {
        element.DescendantNodes().Where(n => n.NodeType == System.Xml.XmlNodeType.Comment).Remove();
    }

    private static void RemoveMetadata(XElement root)
    {
        XNamespace svgNs = "http://www.w3.org/2000/svg";
        root.Descendants(svgNs + "metadata").Remove();
    }

}
