namespace Atlas.Application.Common.Interfaces;

public interface IProactiveAgentService
{
    Task<string> ExplainErrorAsync(string errorMessage, string? stackTrace, CancellationToken ct);
    Task<string> SuggestCommitMessageAsync(string diffContent, CancellationToken ct);
    Task<string> SummarizePrAsync(string prDiff, string prTitle, CancellationToken ct);
    Task<List<string>> AnalyzeDependenciesAsync(string projectFilePath, CancellationToken ct);
}

