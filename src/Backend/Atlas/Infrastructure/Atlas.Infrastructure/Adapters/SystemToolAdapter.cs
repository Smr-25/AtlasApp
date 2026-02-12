using System.Diagnostics;
using System.Runtime.InteropServices;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SystemTools.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class SystemToolAdapter(ILogger<SystemToolAdapter> logger) : ISystemToolAdapter
{
    public async Task<ProcessInfoDto> GetProcessByPortAsync(int port, CancellationToken ct)
    {
        logger.LogInformation("Scanning port {Port} for active processes...", port);

        var pid = 0;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            pid = await GetPidFromWindows(port);

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            pid = await GetPidFromUnix(port);
        

        if (pid == 0)
            return new ProcessInfoDto(0, "Not Found", port, false);
        

        try
        {
            var process = Process.GetProcessById(pid);
            return new ProcessInfoDto(pid, process.ProcessName, port, true);
        }
        catch (Exception)
        {
            return new ProcessInfoDto(pid, "Unknown/System", port, true);
        }
    }

    public Task KillProcessAsync(int pid, CancellationToken ct)
    {
        try
        {
            var process = Process.GetProcessById(pid);
            process.Kill();
            logger.LogInformation("Successfully killed process {Pid}", pid);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to kill process {Pid}", pid);
            throw new Exception($"Could not kill process {pid}. It might require sudo/admin rights.");
        }
    }

    private async Task<int> GetPidFromWindows(int port)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = $"/c netstat -ano | findstr :{port}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null) return 0;

        var output = await process.StandardOutput.ReadToEndAsync();

        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length <= 4 || !parts[1].EndsWith($":{port}")) continue;
            if (int.TryParse(parts.Last().Trim(), out var pid)) return pid;

        }

        return 0;
    }

    private async Task<int> GetPidFromUnix(int port)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"lsof -t -i:{port}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null) return 0;

        var output = await process.StandardOutput.ReadToEndAsync();

        return int.TryParse(output.Trim(), out var pid) ? pid : 0;
    }
}