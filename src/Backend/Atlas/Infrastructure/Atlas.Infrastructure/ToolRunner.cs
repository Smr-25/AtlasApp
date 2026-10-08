using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Atlas.Application;
using Atlas.Infrastructure.Tools;
using Markdig;

namespace Atlas.Infrastructure;

public sealed class ToolRunner
{
    private readonly DeveloperTools dev = new();
    private readonly SystemTools security = new();
    private readonly TextTools text = new();
    private readonly FileTools files = new();
    private readonly ImageTools images = new();

    public async Task<ToolResult> Run(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        if (r.Input.Length + r.Secondary.Length > 8_000_000) throw new ArgumentException("Text tools accept up to 8 million characters.");
        string Serialize(object value) => JsonSerializer.Serialize(value, new JsonSerializerOptions(ToolJson.Options) { WriteIndented = true });
        if (r.Tool.StartsWith("containers.")) return await new ContainerTools().Run(r, report, ct);
        if (r.Tool.StartsWith("pdf.")) return await new PdfTools().Run(r, report, ct);
        switch (r.Tool)
        {
            case "scripts.run": return await new AutomationTools().Script(r, report, ct);
            case "tech.http": return await new AutomationTools().Http(r, ct);
            case "json": return new(r.Option("mode", "format") == "minify" ? dev.MinifyJson(r.Input) : dev.FormatJson(r.Input));
            case "text.clean": return new(text.Clean(r.Input, r.Option("mode", "trim"), r.Option("find"), r.Option("replace")));
            case "text.diff": return new(text.Diff(r.Input, r.Secondary));
            case "text.workflow": return new(text.Workflow(r));
            case "text.csv": return text.Csv(r);
            case "text.markdown": return new(Markdown.ToHtml(r.Input, new MarkdownPipelineBuilder().DisableHtml().Build()));
            case "text.base64": return new(r.Option("mode", "encode") == "decode" ? dev.DecodeBase64(r.Input) : dev.EncodeBase64(r.Input));
            case "text.url": return new(r.Option("mode", "encode") == "decode" ? Uri.UnescapeDataString(r.Input) : Uri.EscapeDataString(r.Input));
            case "tech.jwt": return new("Decoded payload — signature NOT verified.\n\n" + dev.FormatJson(dev.DecodeJwt(r.Input)));
            case "tech.regex": return new(Serialize(dev.TestRegex(r.Secondary, r.Input)));
            case "tech.hash": return new(security.GenerateHash(r.Input, r.Option("algorithm", "SHA256")));
            case "tech.uuid": return new(string.Join('\n', Enumerable.Range(0, r.Number("count", 1, 1, 1000)).Select(_ => Guid.NewGuid().ToString())));
            case "tech.time":
                if (long.TryParse(r.Input.Trim(), out var unix))
                    return new(DateTimeOffset.FromUnixTimeSeconds(unix).ToString("O"));
                if (!DateTimeOffset.TryParse(r.Input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var date))
                    throw new ArgumentException("Enter Unix seconds or an ISO date with timezone.");
                return new(date.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
            case "tech.dns":
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct))
                { timeout.CancelAfter(TimeSpan.FromSeconds(15)); return new(string.Join('\n', await security.LookupIpDnsAsync(r.Input, timeout.Token))); }
            case "tech.tls":
                using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct))
                { timeout.CancelAfter(TimeSpan.FromSeconds(15)); return new(Serialize(await security.CheckSslAsync(r.Input, timeout.Token))); }
            case "tech.ports": return new(await LocalPorts(ct));
            case "files.rename": return await files.Rename(r, report, ct);
            case "files.undo": return await files.UndoRename(r, ct);
            case "files.info": case "files.duplicates": return await files.Inspect(r, report, ct);
            case "files.zip": case "files.unzip": return await files.Archive(r, report, ct);
            case "images.batch": return await images.Batch(r, report, ct);
            case "images.metadata": return await images.Metadata(r, ct);
            case "images.palette":
                await using (var stream = File.OpenRead(FileTools.Inputs(r).Single()))
                    return new(string.Join('\n', images.ExtractColorsFromImage(stream, r.Number("count", 5, 1, 16))));
            case "images.contrast": return new(Serialize(images.CheckContrast(r.Input, r.Secondary)));
            default: throw new ArgumentException("Unknown tool: " + r.Tool);
        }
    }

    private static async Task<string> LocalPorts(CancellationToken ct)
    {
        if (!OperatingSystem.IsMacOS()) throw new PlatformNotSupportedException("Port inspection requires macOS.");
        using var process = new Process { StartInfo = new ProcessStartInfo("/usr/sbin/lsof")
        { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false } };
        foreach (var argument in new[] { "-nP", "-iTCP", "-sTCP:LISTEN" }) process.StartInfo.ArgumentList.Add(argument);
        process.Start();
        var stdout = process.StandardOutput.ReadToEndAsync(ct);
        var stderr = process.StandardError.ReadToEndAsync(ct);
        try
        {
            await process.WaitForExitAsync(ct);
            var output = await stdout; var error = await stderr;
            if (process.ExitCode > 1) throw new IOException(error);
            return string.IsNullOrWhiteSpace(output) ? "No listening TCP ports visible to this user." : output;
        }
        finally { if (!process.HasExited) process.Kill(true); }
    }
}
