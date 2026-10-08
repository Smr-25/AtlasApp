using System.Diagnostics;
using System.Text;

namespace Atlas.Infrastructure;

public record ProcessOutput(int ExitCode, string Output, string Error);
public static class ProcessRunner
{
    public static string Find(string name)
    {
        foreach (var folder in new[] { "/opt/homebrew/bin", "/usr/local/bin", "/usr/bin", "/bin", "/opt/homebrew/lib/docker/cli-plugins", "/usr/local/lib/docker/cli-plugins" })
        {
            var path = Path.Combine(folder, name);
            if (File.Exists(path)) return path;
        }
        throw new FileNotFoundException($"{name} is not installed. Use the runtime setup action first.");
    }

    public static async Task<ProcessOutput> Run(string executable, IEnumerable<string> arguments, string? directory,
        CancellationToken ct, Action<string>? output = null, Dictionary<string, string>? environment = null, bool cleanEnvironment = false)
    {
        ct.ThrowIfCancellationRequested();
        using var process = new Process { StartInfo = new ProcessStartInfo(executable)
        { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false, WorkingDirectory = directory ?? Environment.CurrentDirectory } };
        if (cleanEnvironment) { process.StartInfo.Environment.Clear(); process.StartInfo.Environment["HOME"] = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); }
        foreach (var argument in arguments) process.StartInfo.ArgumentList.Add(argument);
        process.StartInfo.Environment["PATH"] = "/opt/homebrew/bin:/usr/local/bin:/usr/bin:/bin:/usr/sbin:/sbin";
        if (environment != null) foreach (var pair in environment) process.StartInfo.Environment[pair.Key] = pair.Value;
        process.Start();
        using var registration = ct.Register(() => { try { if (!process.HasExited) process.Kill(true); } catch (InvalidOperationException) { } });
        async Task<string> Drain(StreamReader reader)
        {
            var result = new StringBuilder(); var buffer = new char[4096]; int count;
            while ((count = await reader.ReadAsync(buffer, ct)) != 0)
            {
                if (result.Length + count > 2_000_000)
                { if (!process.HasExited) process.Kill(true); throw new IOException("Command output exceeded 2 MB. Narrow the request."); }
                var chunk = new string(buffer, 0, count); result.Append(chunk); output?.Invoke(chunk);
            }
            return result.ToString();
        }
        try
        {
            var stdout = Drain(process.StandardOutput); var stderr = Drain(process.StandardError);
            await Task.WhenAll(stdout, stderr); await process.WaitForExitAsync(ct); ct.ThrowIfCancellationRequested();
            return new(process.ExitCode, await stdout, await stderr);
        }
        finally { if (!process.HasExited) { process.Kill(true); await process.WaitForExitAsync(CancellationToken.None); } }
    }
}
