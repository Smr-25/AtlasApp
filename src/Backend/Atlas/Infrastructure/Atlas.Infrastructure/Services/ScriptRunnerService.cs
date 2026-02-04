using System.Diagnostics;
using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class ScriptRunnerService : IScriptRunnerService
{
    public async Task<string> ExecuteAsync(string command, string arguments, string workingDirectory, CancellationToken cancellationToken = default)
    {
        
        if (string.IsNullOrWhiteSpace(workingDirectory) || !Directory.Exists(workingDirectory))
            workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
            RedirectStandardOutput = true, 
            RedirectStandardError = true, 
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        try{
            process.Start();
            
            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            return !string.IsNullOrEmpty(error) ? $"Output:\n{output} Error:\n{error}" : output;
        }
        catch (Exception ex)
        {
            return $"Execution Failed: {ex.Message}";
        }
    }
}