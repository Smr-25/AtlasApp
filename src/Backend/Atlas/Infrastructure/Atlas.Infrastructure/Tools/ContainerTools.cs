using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Atlas.Application;
using YamlDotNet.RepresentationModel;

namespace Atlas.Infrastructure.Tools;

public record LocalApp(string Id, string Name, string Source, string Context, string Review, string State = "Not checked", string Url = "", string Digest = "");
public sealed class ContainerTools
{
    private static readonly string Root = Environment.GetEnvironmentVariable("ATLAS_APPS_ROOT") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Atlas", "Apps");
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static string Json(object value) => JsonSerializer.Serialize(value, ToolJson.Options);
    private static string AppFolder(string id)
    {
        if (!Guid.TryParseExact(id, "N", out _)) throw new ArgumentException("Invalid application ID.");
        return Path.Combine(Root, id);
    }
    private static Task<ProcessOutput> Compose(string folder, string context, IEnumerable<string> args, CancellationToken ct, Action<string>? report = null) =>
        ProcessRunner.Run(ProcessRunner.Find("docker-compose"), new[] { "--project-name", "atlas-" + Path.GetFileName(folder), "-f", Path.Combine(folder, "compose.json") }.Concat(args), folder, ct, report, new() { ["DOCKER_CONTEXT"] = context, ["COMPOSE_DISABLE_ENV_FILE"] = "true" }, true);
    private static void Success(ProcessOutput output)
    { if (output.ExitCode != 0) throw new IOException(string.IsNullOrWhiteSpace(output.Error) ? output.Output : output.Error); }

    public async Task<ToolResult> Run(ToolRequest r, Action<ToolEvent> report, CancellationToken ct)
    {
        Directory.CreateDirectory(Root);
        var context = r.Option("context", "colima-atlas");
        if (context.StartsWith('-') || context.Length > 120 || !Regex.IsMatch(context, "^[a-zA-Z0-9_.-]+$")) throw new ArgumentException("Choose a valid Docker context.");
        if (r.Tool == "containers.runtime")
        {
            var action = r.Option("action", "status");
            if (action == "contexts")
            {
                var result = await ProcessRunner.Run(ProcessRunner.Find("docker"), ["context", "ls", "--format", "{{.Name}}"], null, ct);
                Success(result); return new(result.Output);
            }
            if (action == "status")
            {
                var result = await ProcessRunner.Run(ProcessRunner.Find("docker"), ["--context", context, "info", "--format", "{{.ServerVersion}}"], null, ct);
                return new(result.ExitCode == 0 ? $"Engine ready · Docker {result.Output.Trim()}" : "Engine unavailable. Start Atlas runtime or select a running Docker context.\n" + result.Error);
            }
            if (!r.Flag("confirmed")) throw new ArgumentException("Confirm the runtime change first.");
            ProcessOutput operation;
            if (action == "setup") operation = await ProcessRunner.Run(ProcessRunner.Find("brew"), ["install", "colima", "docker", "docker-compose"], null, ct, s => report(new("progress", s)));
            else if (action == "start") operation = await ProcessRunner.Run(ProcessRunner.Find("colima"), ["start", "atlas", "--cpu", "2", "--memory", "4", "--disk", "30", "--runtime", "docker", "--activate=false"], null, ct, s => report(new("progress", s)));
            else if (action == "stop") operation = await ProcessRunner.Run(ProcessRunner.Find("colima"), ["stop", "atlas"], null, ct, s => report(new("progress", s)));
            else throw new ArgumentException("Unknown runtime action.");
            Success(operation); return new(operation.Output + operation.Error);
        }
        if (r.Tool == "containers.list")
        {
            var apps = new List<LocalApp>();
            foreach (var manifest in Directory.EnumerateFiles(Root, "app.json", SearchOption.AllDirectories))
            {
                ct.ThrowIfCancellationRequested();
                var app = JsonSerializer.Deserialize<LocalApp>(await File.ReadAllTextAsync(manifest, ct), ToolJson.Options)!;
                try
                {
                    var status = await Compose(Path.GetDirectoryName(manifest)!, app.Context, ["ps", "--all", "--format", "json"], ct);
                    var entries = ParseStatus(status.Output);
                    var states = entries.Select(x => x["State"]?.ToString() ?? "unknown").Distinct().ToArray();
                    app = app with { State = status.ExitCode != 0 ? "Engine unavailable" : states.Length == 0 ? "Not started" : string.Join(", ", states) };
                }
                catch (Exception ex) when (ex is not OperationCanceledException) { app = app with { State = "Unavailable" }; }
                apps.Add(app);
            }
            return new(Json(apps));
        }
        if (r.Tool == "containers.import") return await Import(r, context, ct);
        var folder = AppFolder(r.Input);
        var saved = JsonSerializer.Deserialize<LocalApp>(await File.ReadAllTextAsync(Path.Combine(folder, "app.json"), ct), ToolJson.Options)!;
        if (r.Tool == "containers.inspect") return new(await File.ReadAllTextAsync(Path.Combine(folder, "compose.json"), ct));
        if (r.Tool != "containers.action") throw new ArgumentException("Unknown container operation.");
        var savedConfig = await File.ReadAllTextAsync(Path.Combine(folder, "compose.json"), ct);
        if (saved.Digest != Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(savedConfig))))
            throw new ArgumentException("This recipe changed outside Atlas. Import and review it again.");
        ValidateCompose(JsonNode.Parse(savedConfig)!.AsObject());
        var command = r.Option("action", "logs");
        if (command != "logs" && !r.Flag("confirmed")) throw new ArgumentException("Review and confirm this application action.");
        string[] arguments = command switch
        {
            "start" => ["up", "--detach", "--wait", "--wait-timeout", "180", "--pull", "missing"],
            "stop" => ["stop"], "restart" => ["restart"], "logs" => ["logs", "--no-color", "--tail", "150"],
            "remove" => ["down"], "erase" => ["down", "--volumes"], _ => throw new ArgumentException("Unknown app action.")
        };
        var resultAction = await Compose(folder, saved.Context, arguments, ct, s => report(new("progress", s)));
        Success(resultAction);
        // Keep the manifest after removal so preserved volumes can be reattached by Start.
        return new(command == "remove" ? "Containers removed. Saved data and app recipe were retained; Start recreates the app." : command == "erase" ? "Containers and this application's named volumes were removed. The recipe is retained." : resultAction.Output + resultAction.Error);
    }

    public static JsonObject[] ParseStatus(string output)
    {
        if (string.IsNullOrWhiteSpace(output)) return [];
        if (output.TrimStart().StartsWith('[')) return JsonNode.Parse(output)!.AsArray().OfType<JsonObject>().ToArray();
        return output.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => JsonNode.Parse(line)!.AsObject()).ToArray();
    }

    public static void ValidateCompose(JsonObject root)
    {
        var services = root["services"]?.AsObject() ?? throw new ArgumentException("Compose needs services.");
        if (services.Count is 0 or > 20) throw new ArgumentException("Use between 1 and 20 services.");
        foreach (var pair in services)
        {
            var service = pair.Value!.AsObject();
            if (string.IsNullOrWhiteSpace(service["image"]?.ToString())) throw new ArgumentException($"{pair.Key}: use a published image. Local builds are not supported.");
            foreach (var blocked in new[] { "build", "privileged", "devices", "device_cgroup_rules", "cap_add", "network_mode", "pid", "ipc", "userns_mode", "uts", "volumes_from", "post_start", "pre_stop", "develop", "use_api_socket", "provider", "credential_spec", "extends", "container_name", "security_opt", "sysctls", "gpus", "cgroup", "cgroup_parent", "runtime", "deploy" })
                if (service.ContainsKey(blocked)) throw new ArgumentException($"{pair.Key}: {blocked} is not supported in Atlas imports.");
            if (service["volumes"] is JsonArray mounts)
                foreach (var mount in mounts)
                {
                    if (mount is not JsonObject m || m["type"]?.ToString() != "volume") throw new ArgumentException("Only named volumes are supported; host folder mounts are not accepted.");
                    if (m["volume"] is JsonObject options && options.ContainsKey("subpath")) throw new ArgumentException("Volume subpaths are not supported.");
                }
            if (service["ports"] is JsonArray ports)
                foreach (var port in ports)
                {
                    if (port is not JsonObject p || (p["host_ip"]?.ToString() != "127.0.0.1" && p["host_ip"]?.ToString() != "::1"))
                        throw new ArgumentException("Published ports must explicitly bind to 127.0.0.1 or ::1.");
                }
        }
        foreach (var group in new[] { "volumes", "networks" })
            if (root[group] is JsonObject resources)
                foreach (var resource in resources)
                    if (resource.Value is JsonObject options && (options.ContainsKey("external") || options.ContainsKey("driver_opts") ||
                        (options["name"] != null && options["name"]!.ToString() != root["name"] + "_" + resource.Key) ||
                        (options["driver"] != null && options["driver"]!.ToString() != (group == "volumes" ? "local" : "bridge"))))
                        throw new ArgumentException("External resources and custom volume drivers are not supported.");
        if (root.ContainsKey("secrets") || root.ContainsKey("configs")) throw new ArgumentException("File-backed secrets/configs are not supported in imported recipes.");
    }

    private async Task<ToolResult> Import(ToolRequest r, string context, CancellationToken ct)
    {
        var id = Guid.NewGuid().ToString("N"); var folder = AppFolder(id); Directory.CreateDirectory(folder);
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()) File.SetUnixFileMode(folder, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        try
        {
            var kind = r.Option("kind", "github"); var source = r.Input.Trim(); string yaml;
            if (kind == "image")
            {
                if (!Regex.IsMatch(source, @"^[a-zA-Z0-9][a-zA-Z0-9._/:@-]{0,250}$")) throw new ArgumentException("Enter a public image reference.");
                yaml = Json(new { services = new { app = new { image = source, ports = new[] { $"127.0.0.1:{r.Number("port", 8080, 1024, 65535)}:{r.Number("targetPort", 8080, 1, 65535)}" } } } });
            }
            else if (kind == "file")
            {
                var path = FileTools.Inputs(r).Single(); if (new FileInfo(path).Length > 1_000_000) throw new ArgumentException("Compose file exceeds 1 MB.");
                yaml = await File.ReadAllTextAsync(path, ct); source = Path.GetFileName(path);
            }
            else
            {
                if (!Uri.TryCreate(source, UriKind.Absolute, out var url) || url.Scheme != "https" || url.Host != "github.com") throw new ArgumentException("Use a public github.com file URL: /owner/repo/blob/ref/path/compose.yaml.");
                var parts = url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5 || parts[2] != "blob") throw new ArgumentException("Select the Compose file on GitHub and paste its /blob/ URL.");
                var raw = "https://raw.githubusercontent.com/" + string.Join('/', parts.Take(2)) + "/" + string.Join('/', parts.Skip(3));
                using var response = await Client.GetAsync(raw, HttpCompletionOption.ResponseHeadersRead, ct); response.EnsureSuccessStatusCode();
                await using var stream = await response.Content.ReadAsStreamAsync(ct); using var reader = new StreamReader(stream);
                var chars = new char[1_000_001]; var read = await reader.ReadBlockAsync(chars, ct);
                if (read > 1_000_000) throw new ArgumentException("Compose file exceeds 1 MB."); yaml = new string(chars, 0, read);
            }
            // Parse before invoking Compose: includes and local file references must never be resolved on the host.
            var parsed = new YamlStream(); parsed.Load(new StringReader(yaml));
            if (parsed.Documents.Count != 1) throw new ArgumentException("Choose one Compose document.");
            RejectReferences(parsed.Documents[0].RootNode, 0);
            var sourcePath = Path.Combine(folder, "source.yaml"); await File.WriteAllTextAsync(sourcePath, yaml, ct);
            if (!OperatingSystem.IsWindows()) File.SetUnixFileMode(sourcePath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            var env = new Dictionary<string, string> { ["DOCKER_CONTEXT"] = context, ["COMPOSE_DISABLE_ENV_FILE"] = "true" };
            var userEnv = JsonSerializer.Deserialize<Dictionary<string, string>>(r.Secondary.Length == 0 ? "{}" : r.Secondary) ?? [];
            foreach (var pair in userEnv)
            {
                if (!Regex.IsMatch(pair.Key, "^[A-Z][A-Z0-9_]*$") || pair.Key.StartsWith("DOCKER") || pair.Key.StartsWith("COMPOSE") || new[] { "PATH", "HOME", "LD_PRELOAD", "DYLD_INSERT_LIBRARIES" }.Contains(pair.Key))
                    throw new ArgumentException("Unsupported environment variable name.");
                env[pair.Key] = pair.Value;
            }
            var normalized = await ProcessRunner.Run(ProcessRunner.Find("docker-compose"), ["--project-name", "atlas-" + id, "-f", sourcePath, "config", "--format", "json"], folder, ct, environment: env, cleanEnvironment: true);
            Success(normalized); var config = JsonNode.Parse(normalized.Output)!.AsObject(); ValidateCompose(config);
            var composePath = Path.Combine(folder, "compose.json"); await File.WriteAllTextAsync(composePath, config.ToJsonString(new JsonSerializerOptions { WriteIndented = true }).Replace("$", "$$"), ct);
            if (!OperatingSystem.IsWindows()) File.SetUnixFileMode(composePath, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            File.Delete(sourcePath);
            var services = config["services"]!.AsObject();
            var review = string.Join('\n', services.Select(x => $"{x.Key}: {x.Value!["image"]}"));
            var publishedPorts = services.SelectMany(x => x.Value?["ports"]?.AsArray() ?? []).OfType<JsonObject>().ToArray();
            review += "\nPorts: " + string.Join(", ", publishedPorts.Select(p => $"{p["host_ip"]}:{p["published"]} → {p["target"]}"));
            review += "\nData stays in application-specific Docker volumes. Closing Atlas leaves applications running.";
            var webPort = publishedPorts.FirstOrDefault(p => p["target"]?.ToString() is "8080" or "80" or "3000") ?? publishedPorts.FirstOrDefault();
            var app = new LocalApp(id, r.Option("name", source), source, context, review, "Not started", webPort == null ? "" : $"http://127.0.0.1:{webPort["published"]}");
            app = app with { Digest = Convert.ToHexString(SHA256.HashData(await File.ReadAllBytesAsync(composePath, ct))) };
            await File.WriteAllTextAsync(Path.Combine(folder, "app.json"), Json(app), ct);
            return new(Json(app));
        }
        catch { Directory.Delete(folder, true); throw; }
    }

    private static void RejectReferences(YamlNode node, int depth)
    {
        if (depth > 30) throw new ArgumentException("Compose nesting is too deep.");
        if (node is YamlMappingNode map)
            foreach (var entry in map.Children)
            {
                if (entry.Key.ToString() is "include" or "extends" or "env_file" or "label_file" or "build" or "secrets" or "configs")
                    throw new ArgumentException("Compose imports must use published images and inline settings; includes, builds and file references are not supported.");
                RejectReferences(entry.Value, depth + 1);
            }
        else if (node is YamlSequenceNode sequence) foreach (var child in sequence.Children) RejectReferences(child, depth + 1);
    }

}
