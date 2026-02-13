using System.Diagnostics;
using System.Text.RegularExpressions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SecurityTools.Dtos;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class SecurityToolAdapter(ILogger<SecurityToolAdapter> logger) : ISecurityToolAdapter
{
    public async Task<List<VulnerabilityReportDto>> ScanProjectAsync(string projectPath, CancellationToken ct)
    {
        var arguments = $"list \"{projectPath}\" package --vulnerable --include-transitive";
        
        logger.LogInformation("Scanning for vulnerabilities: dotnet {Args}", arguments);

        var output = await RunDotnetCommandAsync(arguments, ct);
        
        return ParseVulnerabilityOutput(output);
    }

    private List<VulnerabilityReportDto> ParseVulnerabilityOutput(string output)
    {
        var reports = new List<VulnerabilityReportDto>();
        var lines = output.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        VulnerabilityReportDto? currentProject = null;
        var currentVulnerabilities = new List<VulnerabilityItemDto>();

        var packageRegex = new Regex(@"^\s*>\s+(?<name>\S+)\s+(?<version>\S+)\s+(?<severity>\S+)\s+(?<url>http\S+)", RegexOptions.Compiled);

        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("Project '"))
            {
                if (currentProject != null)
                {
                    reports.Add(currentProject with { Vulnerabilities = new List<VulnerabilityItemDto>(currentVulnerabilities) });
                    currentVulnerabilities.Clear();
                }

                var projectName = line.Split('\'')[1]; 
                currentProject = new VulnerabilityReportDto(projectName, new List<VulnerabilityItemDto>());
            }

            var match = packageRegex.Match(line);
            if (match.Success)
            {
                currentVulnerabilities.Add(new VulnerabilityItemDto(
                    PackageName: match.Groups["name"].Value,
                    InstalledVersion: match.Groups["version"].Value,
                    Severity: match.Groups["severity"].Value,
                    AdvisoryUrl: match.Groups["url"].Value
                ));
            }
        }

        if (currentProject != null && currentVulnerabilities.Count > 0)
            reports.Add(currentProject with { Vulnerabilities = [..currentVulnerabilities] });
        

        return reports;
    }
    private async Task<string> RunDotnetCommandAsync(string arguments, CancellationToken ct)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            if (process == null) throw new Exception("Could not start dotnet process");

            var output = await process.StandardOutput.ReadToEndAsync(ct);
            var error = await process.StandardError.ReadToEndAsync(ct);
            
            await process.WaitForExitAsync(ct);

            if (string.IsNullOrEmpty(error) || !error.Contains("error")) return output;
            logger.LogWarning("Dotnet command output error stream: {Error}", error);
            
            return string.IsNullOrWhiteSpace(output) ? throw new Exception(error) : output;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to run security scan. Ensure .NET SDK is installed. Error: {ex.Message}");
        }
    }
}