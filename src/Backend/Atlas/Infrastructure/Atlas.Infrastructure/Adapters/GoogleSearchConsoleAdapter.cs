using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class GoogleSearchConsoleAdapter(IHttpClientFactory httpClientFactory) : IGoogleSearchConsoleAdapter
{
    public Task<List<SearchAnalyticsRow>> GetSearchAnalyticsAsync(string siteUrl, DateTime from, DateTime to, CancellationToken ct)
        => Task.FromResult(new List<SearchAnalyticsRow>());

    public Task<List<string>> GetSitemapsAsync(string siteUrl, CancellationToken ct)
        => Task.FromResult(new List<string>());
}

