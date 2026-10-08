using System.Net.Http;
using System.Text;
using System.Text.Json;
using Atlas.Application;

namespace Atlas.Infrastructure.Tools;

public sealed class AutomationTools
{
    public async Task<ToolResult> Script(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        if (!Path.IsPathFullyQualified(r.Input) || !File.Exists(r.Input)) throw new ArgumentException("Choose an absolute executable path, such as /bin/zsh.");
        if (!Directory.Exists(r.OutputDirectory)) throw new ArgumentException("Choose an existing working folder.");
        var args = JsonSerializer.Deserialize<string[]>(r.Secondary) ?? [];
        if (args.Length > 100) throw new ArgumentException("Use at most 100 arguments.");
        if (!r.Flag("confirmed")) return new($"Executable: {r.Input}\nFolder: {r.OutputDirectory}\nArguments:\n{JsonSerializer.Serialize(args, new JsonSerializerOptions { WriteIndented = true })}\n\nReview before running.");
        var result = await ProcessRunner.Run(r.Input, args, r.OutputDirectory, ct, chunk => report(new("progress", chunk)));
        var text = $"Exit code: {result.ExitCode}\n\n{result.Output}\n{result.Error}";
        if (result.ExitCode != 0) throw new IOException(text);
        return new(text);
    }

    public async Task<ToolResult> Http(ToolRequest r, CancellationToken ct)
    {
        if (!Uri.TryCreate(r.Input.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new ArgumentException("Enter an absolute HTTP or HTTPS URL.");
        using var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(30) };
        using var request = new HttpRequestMessage(new HttpMethod(r.Option("method", "GET")), uri);
        if (!string.IsNullOrEmpty(r.Secondary)) request.Content = new StringContent(r.Secondary, Encoding.UTF8, r.Option("contentType", "application/json"));
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(r.Option("headers", "{}")) ?? [];
        foreach (var header in headers)
            if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value))
                throw new ArgumentException($"Unsupported request header: {header.Key}. Set content type with its dedicated option.");
        var started = System.Diagnostics.Stopwatch.StartNew();
        using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var buffer = new MemoryStream(); var bytes = new byte[65536]; int count;
        while ((count = await stream.ReadAsync(bytes, ct)) > 0)
        { if (buffer.Length + count > 2_000_000) throw new IOException("Response exceeds 2 MB."); buffer.Write(bytes, 0, count); }
        return new($"HTTP {(int)response.StatusCode} {response.ReasonPhrase} · {started.ElapsedMilliseconds} ms\n\n{response.Headers}{response.Content.Headers}\n{Encoding.UTF8.GetString(buffer.ToArray())}");
    }
}
