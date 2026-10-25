using System.IO.Compression;
using Atlas.Application;
using Atlas.Infrastructure;

internal static class FileSafetyTests
{
    public static async Task Run(string temporaryRoot)
    {
        var runner = new ToolRunner();
        var cases = new (string Name, Func<string, Task> Run)[]
        {
            ("rename preflight preserves the entire batch on a late collision", async root =>
            {
                var a = Write(root, "a.txt", "first"); var b = Write(root, "b.txt", "second");
                var occupied = Write(root, "new-b.txt", "existing");
                await Reject<IOException>(() => Run(new("files.rename", Paths: [a, b], OutputDirectory: root, Options: new() { ["prefix"] = "new-", ["apply"] = "true" })));
                Check(File.ReadAllText(a) == "first" && File.ReadAllText(b) == "second" && File.ReadAllText(occupied) == "existing", "Collision changed an input");
                Check(!File.Exists(Path.Combine(root, "new-a.txt")) && !File.Exists(Path.Combine(root, "Atlas-rename-undo.json")), "Collision produced a partial rename/journal");
            }),
            ("rename cancellation rolls back files already moved", async root =>
            {
                var a = Write(root, "a.txt", "first"); var b = Write(root, "b.txt", "second");
                using var stop = new CancellationTokenSource(); var progress = 0;
                await Reject<OperationCanceledException>(() => runner.Run(new("files.rename", Paths: [a, b], OutputDirectory: root, Options: new() { ["prefix"] = "new-", ["apply"] = "true" }), e => { if (e.Type == "progress") { progress++; stop.Cancel(); } }, stop.Token));
                Check(progress == 1 && File.ReadAllText(a) == "first" && File.ReadAllText(b) == "second", "Cancellation did not roll back the first move");
                Check(!Directory.EnumerateFiles(root, "new-*").Any(), "Renamed files survived rollback");
                await Run(new("files.undo", Paths: [Path.Combine(root, "Atlas-rename-undo.json")]));
            }),
            ("undo validates the whole batch before restoring any file", async root =>
            {
                var a = Write(root, "a.txt", "first"); var b = Write(root, "b.txt", "second");
                var renamed = await Run(new("files.rename", Paths: [a, b], OutputDirectory: root, Options: new() { ["prefix"] = "new-", ["apply"] = "true" }));
                Write(root, "b.txt", "replacement");
                await Reject<IOException>(() => Run(new("files.undo", Paths: renamed.Files)));
                Check(!File.Exists(a) && File.ReadAllText(b) == "replacement" && File.Exists(Path.Combine(root, "new-a.txt")) && File.Exists(Path.Combine(root, "new-b.txt")), "Failed undo partially restored or overwrote files");
            }),
            ("undo is safe to repeat", async root =>
            {
                var a = Write(root, "a.txt", "original");
                var renamed = await Run(new("files.rename", Paths: [a], OutputDirectory: root, Options: new() { ["prefix"] = "new-", ["apply"] = "true" }));
                await Run(new("files.undo", Paths: renamed.Files)); await Run(new("files.undo", Paths: renamed.Files));
                Check(File.ReadAllText(a) == "original" && !File.Exists(Path.Combine(root, "new-a.txt")), "Repeated undo changed data");
            }),
            ("rename rejects path separators without moving the source", async root =>
            {
                var a = Write(root, "a.txt", "original");
                foreach (var prefix in new[] { "../", "sub/", "sub\\", "bad:", "bad\0" })
                    await Reject<ArgumentException>(() => Run(new("files.rename", Paths: [a], OutputDirectory: root, Options: new() { ["prefix"] = prefix, ["apply"] = "true" })));
                Check(File.ReadAllText(a) == "original" && Directory.GetFiles(root).Length == 1, "Invalid name changed the source");
            }),
            ("ZIP creation never overwrites an existing output", async root =>
            {
                var a = Write(root, "a.txt", "payload"); var existing = Write(root, "Atlas-archive.zip", "keep this");
                Directory.CreateDirectory(Path.Combine(root, "Atlas-archive-2.zip"));
                var result = await Run(new("files.zip", Paths: [a], OutputDirectory: root));
                Check(File.ReadAllText(existing) == "keep this" && result.Files!.Single() == Path.Combine(root, "Atlas-archive-3.zip"), "Output collision overwrote data");
                using var zip = ZipFile.OpenRead(result.Files![0]);
                using var reader = new StreamReader(zip.GetEntry("a.txt")!.Open());
                Check(await reader.ReadToEndAsync() == "payload", "ZIP payload changed");
            }),
            ("ZIP basename collisions are rejected before writing", async root =>
            {
                var left = Directory.CreateDirectory(Path.Combine(root, "left")).FullName;
                var right = Directory.CreateDirectory(Path.Combine(root, "right")).FullName;
                var a = Write(left, "same.txt", "left"); var b = Write(right, "same.txt", "right");
                await Reject<IOException>(() => Run(new("files.zip", Paths: [a, b], OutputDirectory: root)));
                Check(!Directory.GetFiles(root).Any() && File.ReadAllText(a) == "left" && File.ReadAllText(b) == "right", "Colliding ZIP mutated data");
            }),
            ("ZIP cancellation removes the unfinished archive", async root =>
            {
                var a = Write(root, "a.txt", "first"); var b = Write(root, "b.txt", "second");
                using var stop = new CancellationTokenSource(); var progress = 0;
                await Reject<OperationCanceledException>(() => runner.Run(new("files.zip", Paths: [a, b], OutputDirectory: root), e => { if (e.Type == "progress") { progress++; stop.Cancel(); } }, stop.Token));
                Check(progress == 1 && Directory.GetFiles(root).Length == 2 && File.Exists(a) && File.Exists(b), "Cancelled ZIP left partial output");
            }),
            ("ZIP validation rejects a late unsafe entry before extracting a safe one", async root =>
            {
                var archive = Zip(root, ("safe.txt", "safe"), ("../escape.txt", "unsafe"));
                await Reject<IOException>(() => Run(new("files.unzip", Paths: [archive], OutputDirectory: root)));
                Check(!Directory.GetDirectories(root).Any() && Directory.GetFiles(root).Length == 1, "Unsafe ZIP partially extracted");
            }),
            ("ZIP extraction failures remove partially extracted data", async root =>
            {
                var archive = Zip(root, ("node", "file"), ("node/child.txt", "cannot create inside a file"));
                await Reject<IOException>(() => Run(new("files.unzip", Paths: [archive], OutputDirectory: root)));
                Check(!Directory.GetDirectories(root).Any() && File.Exists(archive), "Failed extraction left partial output");
            }),
            ("ZIP extraction cancellation removes partial output and preserves source", async root =>
            {
                var archive = Zip(root, ("first.txt", "first"), ("second.txt", "second"));
                var bytes = File.ReadAllBytes(archive); using var stop = new CancellationTokenSource(); var progress = 0;
                await Reject<OperationCanceledException>(() => runner.Run(new("files.unzip", Paths: [archive], OutputDirectory: root), e => { if (e.Type == "progress") { progress++; stop.Cancel(); } }, stop.Token));
                Check(progress == 1 && !Directory.GetDirectories(root).Any() && File.ReadAllBytes(archive).SequenceEqual(bytes), "Cancelled extraction left data or changed source");
            }),
            ("ZIP extraction preserves an existing destination folder", async root =>
            {
                var archive = Zip(root, ("nested/file.txt", "new data"));
                var occupied = Directory.CreateDirectory(Path.Combine(root, "sample-extracted")).FullName;
                var sentinel = Write(occupied, "keep.txt", "old data");
                var result = await Run(new("files.unzip", Paths: [archive], OutputDirectory: root));
                Check(result.Files![0] != occupied && File.ReadAllText(sentinel) == "old data" && File.ReadAllText(Path.Combine(result.Files[0], "nested/file.txt")) == "new data", "Extraction reused an occupied directory");
            })
        };
        foreach (var test in cases)
        {
            var root = Path.Combine(temporaryRoot, "safety-" + Guid.NewGuid()); Directory.CreateDirectory(root);
            try { await test.Run(root); Console.WriteLine("PASS: " + test.Name); }
            catch (Exception error) { throw new Exception("FAIL: " + test.Name, error); }
            finally { Directory.Delete(root, true); }
        }
        Task<ToolResult> Run(ToolRequest request) => runner.Run(request, _ => { }, CancellationToken.None);
    }

    private static string Write(string folder, string name, string text)
    { var path = Path.Combine(folder, name); File.WriteAllText(path, text); return path; }
    private static string Zip(string folder, params (string Name, string Text)[] entries)
    {
        var path = Path.Combine(folder, "sample.zip");
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        foreach (var entry in entries) { using var writer = new StreamWriter(archive.CreateEntry(entry.Name).Open()); writer.Write(entry.Text); }
        return path;
    }
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static async Task Reject<T>(Func<Task<ToolResult>> action) where T : Exception
    { try { await action(); } catch (T) { return; } throw new Exception("Expected " + typeof(T).Name); }
}
