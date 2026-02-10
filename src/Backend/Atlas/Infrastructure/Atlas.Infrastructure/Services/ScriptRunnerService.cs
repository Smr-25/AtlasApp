using System.Diagnostics;
using Atlas.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class ScriptRunnerService(ILogger<ScriptRunnerService> logger) : IScriptRunnerService
{
    public async Task<string> ExecuteAsync(string command, string arguments, string workingDirectory, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(workingDirectory) || !Directory.Exists(workingDirectory))
            workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        
        logger.LogInformation("Executing command: {Command} {Arguments} in {WorkingDirectory}", command, arguments, workingDirectory);
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory, 
            RedirectStandardOutput = true, 
            RedirectStandardError = true, 
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        try
        {
            process.Start();
            
            var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
            var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

            await process.WaitForExitAsync(cancellationToken);

            var output = await outputTask;
            var error = await errorTask;

            if (!string.IsNullOrEmpty(error))
            {
                logger.LogWarning("Command completed with errors: {Error}", error);
                return $"Output:\n{output} Error:\n{error}";
            }
            
            logger.LogDebug("Command completed successfully");
            return output;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Command execution failed: {Command} {Arguments}", command, arguments);
            return $"Execution Failed: {ex.Message}";
        }
    }
}