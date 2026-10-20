using System.IO.Compression;
using Atlas.Application;
using Atlas.Infrastructure;
using Atlas.Infrastructure.Tools;
using System.Text.Json.Nodes;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

internal static class SelfTest
{
    public static async Task Run()
    {
        var runner = new ToolRunner();
        var root = Path.Combine(Path.GetTempPath(), "atlas-test-" + Guid.NewGuid());
        Directory.CreateDirectory(root);
        Task<ToolResult> Run(ToolRequest r) => runner.Run(r, _ => { }, CancellationToken.None);
        void Check(bool value, string message) { if (!value) throw new Exception(message); }
        async Task Reject(ToolRequest r) { try { await Run(r); } catch { return; } throw new Exception("Expected rejection: " + r.Tool); }
        try
        {
            Check((await Run(new("json", "{\"a\":1}"))).Text.Contains("\n"), "JSON format");
            Check((await Run(new("json", "{ \"a\": 1 }", Options: new() { ["mode"] = "minify" }))).Text == "{\"a\":1}", "JSON minify");
            await Reject(new("json", "{bad}"));
            await Reject(new("json", "{'a':1}"));
            await Reject(new("json", ""));
            Check((await Run(new("text.base64", "Salam 🌍"))).Text == "U2FsYW0g8J+MjQ==", "Unicode Base64");
            Check((await Run(new("text.workflow", " a \n\n a "))).Text == "a", "Text workflow order");
            Check((await Run(new("text.csv", "a,b\n\"x,y\",\"line\nnext\""))).Rows![1][0] == "x,y", "Quoted multiline CSV");
            Check((await Run(new("text.diff", "a\nb", "a\nc"))).Text.Contains("− b"), "Diff");
            Check((await Run(new("tech.hash", "abc"))).Text == "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", "SHA256 vector");
            await Reject(new("tech.jwt", "invalid"));
            await Reject(new("images.contrast", "bad color", "#fff"));
            var a = Path.Combine(root, "a.txt"); var b = Path.Combine(root, "b.txt");
            await File.WriteAllTextAsync(a, "same"); await File.WriteAllTextAsync(b, "same");
            var preview = await Run(new("files.rename", Paths: [a], Options: new() { ["prefix"] = "new-" }));
            Check(File.Exists(a) && preview.Rows!.Length == 2, "Rename preview must not mutate");
            await Reject(new("files.rename", Paths: [a], Options: new() { ["find"] = "a", ["replace"] = "b", ["apply"] = "true" }, OutputDirectory: root));
            Check(File.Exists(a), "Collision must preserve source");
            using (var cancelled = new CancellationTokenSource())
            {
                cancelled.Cancel();
                try { await runner.Run(new("files.rename", Paths: [a], OutputDirectory: root, Options: new() { ["prefix"] = "stopped-", ["apply"] = "true" }), _ => { }, cancelled.Token); throw new Exception("Cancelled rename ran"); }
                catch (OperationCanceledException) { Check(File.Exists(a), "Cancelled rename preserves source"); }
            }
            var renamed = await Run(new("files.rename", Paths: [a], OutputDirectory: root, Options: new() { ["prefix"] = "new-", ["apply"] = "true" }));
            Check(!File.Exists(a), "Rename apply");
            var renamedPath = Path.Combine(root, "new-a.txt");
            await File.WriteAllTextAsync(renamedPath, "changed");
            await Reject(new("files.undo", Paths: renamed.Files));
            Check(File.Exists(renamedPath) && !File.Exists(a), "Modified file undo does not mutate");
            await File.WriteAllTextAsync(renamedPath, "same");
            await Run(new("files.undo", Paths: renamed.Files)); Check(File.Exists(a), "Undo restore");
            Check((await Run(new("files.duplicates", Paths: [a, b]))).Rows!.Length == 3, "Duplicate content");
            var archive = await Run(new("files.zip", Paths: [a, b], OutputDirectory: root));
            var unpacked = await Run(new("files.unzip", Paths: archive.Files, OutputDirectory: root));
            Check(File.ReadAllText(Path.Combine(unpacked.Files![0], "a.txt")) == "same", "ZIP round trip");
            var malicious = Path.Combine(root, "bad.zip");
            using (var zip = ZipFile.Open(malicious, ZipArchiveMode.Create)) zip.CreateEntry("../escape.txt");
            await Reject(new("files.unzip", Paths: [malicious], OutputDirectory: root));
            Check(!File.Exists(Path.Combine(root, "escape.txt")), "Traversal cannot write outside extraction root");
            var sibling = Path.Combine(root, "sibling.zip");
            using (var zip = ZipFile.Open(sibling, ZipArchiveMode.Create)) zip.CreateEntry("../sibling-extracted-other/escape.txt");
            await Reject(new("files.unzip", Paths: [sibling], OutputDirectory: root));
            Check(!Directory.Exists(Path.Combine(root, "sibling-extracted-other")), "Sibling prefix is not an extraction child");
            var absolute = Path.Combine(root, "absolute.zip");
            using (var zip = ZipFile.Open(absolute, ZipArchiveMode.Create)) zip.CreateEntry(Path.Combine(root, "absolute-escape.txt"));
            await Reject(new("files.unzip", Paths: [absolute], OutputDirectory: root));
            Check(!File.Exists(Path.Combine(root, "absolute-escape.txt")), "Absolute archive paths cannot write files");
            var symbolic = Path.Combine(root, "symlink.zip");
            using (var zip = ZipFile.Open(symbolic, ZipArchiveMode.Create)) zip.CreateEntry("link").ExternalAttributes = 0xA000 << 16;
            await Reject(new("files.unzip", Paths: [symbolic], OutputDirectory: root));
            var duplicate = Path.Combine(root, "duplicate.zip");
            using (var zip = ZipFile.Open(duplicate, ZipArchiveMode.Create)) { zip.CreateEntry("same"); zip.CreateEntry("SAME"); }
            await Reject(new("files.unzip", Paths: [duplicate], OutputDirectory: root));
            var imagePath = Path.Combine(root, "red.png");
            using (var image = new Image<Rgba32>(40, 20, new Rgba32(255, 0, 0))) await image.SaveAsPngAsync(imagePath);
            Check((await Run(new("images.palette", Paths: [imagePath]))).Text.StartsWith("#F00000"), "Palette reads pixels");
            var resized = await Run(new("images.batch", Paths: [imagePath], OutputDirectory: root, Options: new() { ["width"] = "20", ["format"] = "png" }));
            using (var image = Image.Load(resized.Files![0])) Check(image.Width == 20 && image.Height == 10, "Resize aspect ratio");
            Check(File.Exists(imagePath), "Image original preserved");
            var partial = await Run(new("images.batch", Paths: [a, imagePath], OutputDirectory: root, Options: new() { ["format"] = "png" }));
            Check(partial.Files!.Length == 1 && partial.Text.Contains("1 failed") && partial.Rows![1][1].StartsWith("Error:"), "Partial image failures remain visible");
            await Reject(new("images.batch", Paths: [a], OutputDirectory: root));
            await Reject(new("tech.regex", new string('a', 10001), "a"));
            using (var pdf = new PdfDocumentBuilder())
            {
                var font = pdf.AddStandard14Font(Standard14Font.Helvetica);
                pdf.AddPage(200, 300).AddText("Atlas first page", 16, new PdfPoint(20, 200), font);
                pdf.AddPage(200, 300).AddText("Atlas second page", 16, new PdfPoint(20, 200), font);
                var source = Path.Combine(root, "source.pdf"); await File.WriteAllBytesAsync(source, pdf.Build());
                var merged = await Run(new("pdf.merge", Paths: [source, source], OutputDirectory: root));
                using (var doc = PdfDocument.Open(merged.Files![0])) Check(doc.NumberOfPages == 4, "PDF merge in C#");
                var selected = await Run(new("pdf.pages", Paths: [source], OutputDirectory: root, Options: new() { ["pages"] = "2,1" }));
                using (var doc = PdfDocument.Open(selected.Files![0])) Check(doc.GetPage(1).Text.Contains("second"), "PDF ordering preserves text");
                Check((await Run(new("pdf.text", Paths: [source]))).Text.Contains("Atlas first page"), "Selectable PDF text");
                await Reject(new("pdf.pages", Paths: [source], OutputDirectory: root, Options: new() { ["pages"] = "0" }));
                Check(PdfTools.Pages("2,1-2", 2).SequenceEqual(new[] { 2, 1, 2 }), "PDF range parser");
                Check((await Run(new("pdf.split", Paths: [source], OutputDirectory: root))).Files!.Length == 2, "PDF split");
            }
            var imagePdf = await Run(new("pdf.images", Paths: [imagePath], OutputDirectory: root));
            using (var doc = PdfDocument.Open(imagePdf.Files![0])) Check(doc.GetPage(1).NumberOfImages == 1, "Image to PDF");
            var valid = JsonNode.Parse("""{"name":"atlas-check","services":{"app":{"image":"nginx:alpine","ports":[{"host_ip":"127.0.0.1","published":"8080","target":80}]}},"volumes":{"data":{"name":"atlas-check_data"}}}""")!.AsObject();
            ContainerTools.ValidateCompose(valid);
            foreach (var field in new[] { "privileged", "build", "network_mode", "use_api_socket", "security_opt" })
            {
                var unsafeConfig = valid.DeepClone().AsObject(); unsafeConfig["services"]!["app"]![field] = "unsafe";
                bool rejected = false; try { ContainerTools.ValidateCompose(unsafeConfig); } catch (ArgumentException) { rejected = true; }
                Check(rejected, "Unsafe Compose option rejected: " + field);
            }
            var external = valid.DeepClone().AsObject(); external["volumes"]!["data"]!["name"] = "other-project-data";
            bool externalRejected = false; try { ContainerTools.ValidateCompose(external); } catch (ArgumentException) { externalRejected = true; }
            Check(externalRejected, "Other project volumes protected");
            Check(ContainerTools.ParseStatus("[{\"State\":\"running\"}]").Length == 1 && ContainerTools.ParseStatus("{\"State\":\"running\"}\n{\"State\":\"exited\"}").Length == 2, "Compose JSON variants");
            if (!OperatingSystem.IsWindows())
            {
                var marker = Path.Combine(root, "command-ran");
                var arguments = System.Text.Json.JsonSerializer.Serialize(new[] { "-c", "printf ok > command-ran; printf stdout; printf stderr >&2" });
                await Run(new("scripts.run", "/bin/sh", arguments, OutputDirectory: root)); Check(!File.Exists(marker), "Preview never executes command");
                var script = await Run(new("scripts.run", "/bin/sh", arguments, OutputDirectory: root, Options: new() { ["confirmed"] = "true" }));
                Check(File.ReadAllText(marker) == "ok" && script.Text.Contains("stdout") && script.Text.Contains("stderr"), "Script cwd and both output streams");
                await Reject(new("scripts.run", "/bin/sh", "[\"-c\",\"exit 7\"]", OutputDirectory: root, Options: new() { ["confirmed"] = "true" }));
                using var stop = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));
                try { await ProcessRunner.Run("/bin/sh", ["-c", "sleep 30; touch must-not-exist"], root, stop.Token); throw new Exception("Script cancellation failed"); }
                catch (OperationCanceledException) { Check(!File.Exists(Path.Combine(root, "must-not-exist")), "Cancelled child cannot finish"); }
            }
            await Reject(new("tech.http", "file:///etc/passwd"));
            Console.WriteLine("PASS: JSON, Unicode, text workflow, CSV, diff, hash, validation, rename preview/collision/apply/undo, duplicates, ZIP safety, image resize and palette, C# PDFs, Compose isolation and command execution/cancellation.");
        }
        finally { Directory.Delete(root, true); }
    }
}
