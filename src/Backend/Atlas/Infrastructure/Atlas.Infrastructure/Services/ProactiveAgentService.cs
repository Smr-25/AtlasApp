using Atlas.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class ProactiveAgentService(IAiService aiService, ILogger<ProactiveAgentService> logger) : IProactiveAgentService
{
    public async Task<string> ExplainErrorAsync(string errorMessage, string? stackTrace, CancellationToken ct)
    {
        var prompt = $"Error: {errorMessage}";
        if (!string.IsNullOrEmpty(stackTrace))
            prompt += $"\n\nStack Trace:\n{stackTrace}";

        return await aiService.GenerateResponseAsync(
            "You are a senior developer. Explain this error in simple language and suggest a fix. Be concise.",
            prompt, ct);
    }

    public async Task<string> SuggestCommitMessageAsync(string diffContent, CancellationToken ct)
    {
        return await aiService.GenerateResponseAsync(
            "You are a git commit message generator. Based on the diff, write a concise conventional commit message. Return only the message, nothing else.",
            diffContent, ct);
    }

    public async Task<string> SummarizePrAsync(string prDiff, string prTitle, CancellationToken ct)
    {
        var prompt = $"PR Title: {prTitle}\n\nDiff:\n{prDiff}";
        return await aiService.GenerateResponseAsync(
            "You are a code reviewer. Summarize this PR in 3 sentences: what changed, why it matters, and any concerns.",
            prompt, ct);
    }

    public async Task<List<string>> AnalyzeDependenciesAsync(string projectFilePath, CancellationToken ct)
    {
        try
        {
            if (!File.Exists(projectFilePath))
                return [$"File not found: {projectFilePath}"];

            var content = await File.ReadAllTextAsync(projectFilePath, ct);
            var warnings = new List<string>();

            if (projectFilePath.EndsWith(".csproj"))
            {
                var lines = content.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("PackageReference"))
                        warnings.Add($"Found dependency: {line.Trim()}");
                }
            }

            if (warnings.Count == 0)
                warnings.Add("No dependencies found or all up to date.");

            return warnings;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to analyze dependencies at {Path}", projectFilePath);
            return [$"Error analyzing dependencies: {ex.Message}"];
        }
    }
}

