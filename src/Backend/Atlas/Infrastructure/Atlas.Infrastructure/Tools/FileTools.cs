using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using Atlas.Application;

namespace Atlas.Infrastructure.Tools;

public sealed class FileTools
{
    public static string[] Inputs(ToolRequest r)
    {
        var paths = (r.Paths ?? []).Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        if (paths.Length == 0) throw new ArgumentException("Choose at least one file.");
        return paths;
    }

    public static string OutputFolder(ToolRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.OutputDirectory) || !Directory.Exists(r.OutputDirectory))
            throw new ArgumentException("Choose an existing output folder.");
        return Path.GetFullPath(r.OutputDirectory);
    }

    public static string UniquePath(string folder, string name)
    {
        name = Path.GetFileName(name);
        var path = Path.Combine(folder, name);
        for (var i = 2; File.Exists(path) || Directory.Exists(path); i++)
            path = Path.Combine(folder, $"{Path.GetFileNameWithoutExtension(name)}-{i}{Path.GetExtension(name)}");
        return path;
    }

    public static IEnumerable<string> Expand(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0) continue;
            if (File.Exists(path)) yield return path;
            else if (Directory.Exists(path))
                foreach (var file in Directory.EnumerateFiles(path, "*", new EnumerationOptions
                { RecurseSubdirectories = true, AttributesToSkip = FileAttributes.ReparsePoint, IgnoreInaccessible = false }))
                    yield return file;
        }
    }

    public static async Task<string> Hash(string path, CancellationToken ct)
    {
        await using var stream = File.OpenRead(path);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream, ct)).ToLowerInvariant();
    }

    public record RenameEntry(string Before, string After, string Hash);

    public async Task<ToolResult> Rename(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        var paths = Inputs(r);
        var rows = new List<string[]> { new[] { "Before", "After" } };
        var plan = new List<RenameEntry>();
        var start = r.Number("start", 1, 0, 999999);
        for (var i = 0; i < paths.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            var path = paths[i];
            if (!File.Exists(path) || (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
                throw new ArgumentException("Rename accepts regular files only.");
            var stem = Path.GetFileNameWithoutExtension(path);
            if (r.Option("find") != "") stem = stem.Replace(r.Option("find"), r.Option("replace"), StringComparison.Ordinal);
            var date = r.Flag("date") ? File.GetLastWriteTime(path).ToString("yyyy-MM-dd") + "-" : "";
            var number = r.Flag("number") ? $"{start + i:D3}-" : "";
            var name = r.Option("prefix") + date + number + stem + r.Option("suffix") + Path.GetExtension(path);
            if (name.IndexOfAny(['/', '\\', '\0', ':']) >= 0 || string.IsNullOrWhiteSpace(name) || name.Length > 240)
                throw new ArgumentException("The resulting filename is invalid or too long.");
            var destination = Path.Combine(Path.GetDirectoryName(path)!, name);
            rows.Add([path, destination]);
            if (path != destination) plan.Add(new(path, destination, ""));
        }
        if (plan.Select(x => x.After).Distinct(StringComparer.OrdinalIgnoreCase).Count() != plan.Count ||
            plan.Any(x => File.Exists(x.After) || Directory.Exists(x.After)))
            throw new IOException("A target name already exists. Change the rules; no files were renamed.");
        if (!r.Flag("apply")) return new($"Preview: {plan.Count} files will be renamed. Originals stay in the same folder.", Rows: rows.ToArray());
        if (plan.Count == 0) return new("All selected files already have the requested names.", Rows: rows.ToArray());
        var journalPath = UniquePath(OutputFolder(r), "Atlas-rename-undo.json");
        for (var i = 0; i < plan.Count; i++) plan[i] = plan[i] with { Hash = await Hash(plan[i].Before, ct) };
        // Write the recovery plan before moving anything, so a crash still leaves an undo record.
        await using (var journal = new FileStream(journalPath, FileMode.CreateNew))
            await JsonSerializer.SerializeAsync(journal, plan, ToolJson.Options, ct);
        var moved = new List<RenameEntry>();
        try
        {
            foreach (var entry in plan)
            {
                ct.ThrowIfCancellationRequested();
                File.Move(entry.Before, entry.After, false);
                moved.Add(entry);
                report(new("progress", Path.GetFileName(entry.After), (double)moved.Count / Math.Max(1, plan.Count)));
            }
        }
        catch
        {
            foreach (var entry in moved.AsEnumerable().Reverse()) File.Move(entry.After, entry.Before, false);
            throw;
        }
        return new($"Renamed {moved.Count} files. Keep the undo record to restore their names.", [journalPath], rows.ToArray());
    }

    public async Task<ToolResult> UndoRename(ToolRequest r, CancellationToken ct)
    {
        var recordPath = Inputs(r).Single();
        if (new FileInfo(recordPath).Length > 16_000_000) throw new ArgumentException("Rename record is too large.");
        var entries = JsonSerializer.Deserialize<List<RenameEntry>>(await File.ReadAllTextAsync(recordPath, ct), ToolJson.Options)
            ?? throw new ArgumentException("Invalid rename record.");
        if (entries.Count > 10000 || entries.Select(x => x.Before).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Count ||
            entries.Select(x => x.After).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Count)
            throw new ArgumentException("Duplicate or excessive rename record entries.");
        var moved = new List<RenameEntry>();
        foreach (var entry in entries)
        {
            if (!Path.IsPathFullyQualified(entry.Before) || !Path.IsPathFullyQualified(entry.After) ||
                Path.GetDirectoryName(entry.Before) != Path.GetDirectoryName(entry.After))
                throw new ArgumentException("Invalid rename record paths.");
            if (File.Exists(entry.Before) && !File.Exists(entry.After)) continue;
            if (File.Exists(entry.Before) || !File.Exists(entry.After) || (File.GetAttributes(entry.After) & FileAttributes.ReparsePoint) != 0 || await Hash(entry.After, ct) != entry.Hash)
                throw new IOException("A file changed or its original name is occupied. Undo stopped without changing files.");
            moved.Add(entry);
        }
        var restored = new List<RenameEntry>();
        try
        {
            foreach (var entry in moved)
            {
                ct.ThrowIfCancellationRequested();
                File.Move(entry.After, entry.Before, false); restored.Add(entry);
            }
        }
        catch
        {
            foreach (var entry in restored.AsEnumerable().Reverse()) File.Move(entry.Before, entry.After, false);
            throw;
        }
        return new($"Restored {restored.Count} filenames.");
    }

    public async Task<ToolResult> Inspect(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        var paths = Expand(Inputs(r)).Distinct().ToArray();
        var rows = new List<string[]> { new[] { "File", "Bytes", "SHA256" } };
        var errors = new List<string>();
        for (var i = 0; i < paths.Length; i++)
        {
            ct.ThrowIfCancellationRequested();
            try { rows.Add([paths[i], new FileInfo(paths[i]).Length.ToString(), await Hash(paths[i], ct)]); }
            catch (IOException ex) { errors.Add($"{paths[i]}: {ex.Message}"); }
            report(new("progress", Path.GetFileName(paths[i]), (double)(i + 1) / paths.Length));
        }
        if (r.Tool == "files.duplicates")
            rows = [rows[0], .. rows.Skip(1).GroupBy(x => x[2]).Where(g => g.Count() > 1).SelectMany(g => g)];
        return new($"{rows.Count - 1} files listed. SHA256 compares file content.\n" + string.Join('\n', errors), Rows: rows.ToArray());
    }

    public async Task<ToolResult> Archive(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        var paths = Inputs(r);
        var folder = OutputFolder(r);
        if (r.Tool == "files.zip")
        {
            var files = new List<(string Path, string Name)>();
            foreach (var input in paths)
                foreach (var file in Expand([input]))
                    files.Add((file, Directory.Exists(input) ? Path.GetFileName(input) + "/" + Path.GetRelativePath(input, file) : Path.GetFileName(file)));
            if (files.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != files.Count)
                throw new IOException("ZIP entry names would collide. Select files with distinct names.");
            var output = UniquePath(folder, "Atlas-archive.zip");
            var temporary = Path.Combine(folder, ".atlas-" + Guid.NewGuid() + ".zip");
            try
            {
                using (var zip = new ZipArchive(new FileStream(temporary, FileMode.CreateNew), ZipArchiveMode.Create))
                {
                for (var i = 0; i < files.Count; i++)
                {
                    ct.ThrowIfCancellationRequested();
                    var entry = zip.CreateEntry(files[i].Name, CompressionLevel.Optimal);
                    await using var source = File.OpenRead(files[i].Path);
                    await using var target = entry.Open();
                    await source.CopyToAsync(target, ct);
                    report(new("progress", files[i].Name, (double)(i + 1) / files.Count));
                }
                }
                ct.ThrowIfCancellationRequested();
                File.Move(temporary, output, false);
            }
            finally { File.Delete(temporary); }
            return new($"Created archive with {files.Count} files.", [output]);
        }
        using var archive = ZipFile.OpenRead(paths.Single());
        if (archive.Entries.Count > 10000 || archive.Entries.Sum(x => x.Length) > 2L * 1024 * 1024 * 1024)
            throw new IOException("This alpha supports ZIP extraction up to 10,000 entries and 2 GB uncompressed.");
        var destination = UniquePath(folder, Path.GetFileNameWithoutExtension(paths[0]) + "-extracted");
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in archive.Entries)
        {
            var target = Path.GetFullPath(Path.Combine(destination, entry.FullName));
            if (!target.StartsWith(destination + Path.DirectorySeparatorChar, StringComparison.Ordinal) ||
                ((entry.ExternalAttributes >> 16) & 0xF000) == 0xA000 || !names.Add(target))
                throw new IOException("Unsafe or duplicate ZIP entry. Nothing was extracted.");
        }
        Directory.CreateDirectory(destination);
        try
        {
            for (var i = 0; i < archive.Entries.Count; i++)
            {
                ct.ThrowIfCancellationRequested();
                var entry = archive.Entries[i];
                var target = Path.GetFullPath(Path.Combine(destination, entry.FullName));
                if (entry.FullName.EndsWith('/')) Directory.CreateDirectory(target);
                else
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                    await using var source = entry.Open();
                    await using var output = new FileStream(target, FileMode.CreateNew);
                    var buffer = new byte[65536];
                    long written = 0;
                    int read;
                    while ((read = await source.ReadAsync(buffer, ct)) > 0)
                    {
                        written += read;
                        if (written > entry.Length) throw new IOException("ZIP entry exceeds its declared size.");
                        await output.WriteAsync(buffer.AsMemory(0, read), ct);
                    }
                }
                report(new("progress", entry.FullName, (double)(i + 1) / archive.Entries.Count));
            }
        }
        catch { Directory.Delete(destination, true); throw; }
        return new($"Extracted {archive.Entries.Count} entries.", [destination]);
    }
}
