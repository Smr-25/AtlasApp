using System.Net.Http.Headers;
using System.Net.Http.Json;
using Atlas.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Adapters;

public class PerplexityAdapter(IHttpClientFactory httpClientFactory, ILogger<PerplexityAdapter> logger) : IPerplexityAdapter
{
    private const string BaseUrl = "https://api.perplexity.ai";

    public async Task<string> SearchAsync(string query, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient("AtlasClient");
        try
        {
            var payload = new
            {
                model = "llama-3.1-sonar-small-128k-online",
                messages = new[] { new { role = "user", content = query } }
            };
            var response = await client.PostAsJsonAsync($"{BaseUrl}/chat/completions", payload, ct);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PerplexityResponse>(ct);
            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "No answer found.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Perplexity search failed for query: {Query}", query);
            return $"Search failed: {ex.Message}";
        }
    }

    public async Task<string> SearchWithContextAsync(string errorMessage, string stackTrace, CancellationToken ct)
    {
        var query = $"I got this error in my code: \"{errorMessage}\"\n\nStack trace:\n{stackTrace}\n\nPlease explain what's wrong and how to fix it.";
        return await SearchAsync(query, ct);
    }

    private record PerplexityResponse(List<PerplexityChoice>? Choices);
    private record PerplexityChoice(PerplexityMessage? Message);
    private record PerplexityMessage(string? Content);
}

