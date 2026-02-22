namespace Atlas.Application.Common.Interfaces;

public interface IPerplexityAdapter
{
    Task<string> SearchAsync(string query, CancellationToken ct);
    Task<string> SearchWithContextAsync(string errorMessage, string stackTrace, CancellationToken ct);
}

