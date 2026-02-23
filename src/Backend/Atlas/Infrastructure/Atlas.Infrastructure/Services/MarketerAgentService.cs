using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Services;

public class MarketerAgentService(HttpClient httpClient) : IMarketerAgentService
{
    public Task<BudgetBleedResult> DetectBudgetBleedAsync(Guid userId, CancellationToken ct)
    {
        return Task.FromResult(new BudgetBleedResult(false, []));
    }

    public async Task<List<BrokenLinkResult>> DetectBrokenLinksAsync(string baseUrl, CancellationToken ct)
    {
        var results = new List<BrokenLinkResult>();
        try
        {
            var response = await httpClient.GetAsync(baseUrl, ct);
            if (!response.IsSuccessStatusCode)
                results.Add(new BrokenLinkResult(baseUrl, (int)response.StatusCode, response.ReasonPhrase));
        }
        catch (Exception ex)
        {
            results.Add(new BrokenLinkResult(baseUrl, 0, ex.Message));
        }
        return results;
    }

    public Task<List<TrendResult>> GetViralTrendsAsync(string industry, CancellationToken ct)
    {
        return Task.FromResult(new List<TrendResult>());
    }

    public Task<List<CompetitorPriceResult>> DetectCompetitorPriceDropAsync(string competitorUrl, CancellationToken ct)
    {
        return Task.FromResult(new List<CompetitorPriceResult>());
    }

    public Task<string> ResendLowOpenRateAsync(string campaignId, string newSubject, CancellationToken ct)
    {
        return Task.FromResult($"Campaign {campaignId} scheduled for resend with subject: {newSubject}");
    }

    public Task<string> AppendUtmAsync(string url, string source, string medium, string campaign, CancellationToken ct)
    {
        var separator = url.Contains('?') ? "&" : "?";
        var utmUrl = $"{url}{separator}utm_source={Uri.EscapeDataString(source)}&utm_medium={Uri.EscapeDataString(medium)}&utm_campaign={Uri.EscapeDataString(campaign)}";
        return Task.FromResult(utmUrl);
    }

    public Task<CartAbandonmentResult> DetectCartAbandonmentAsync(Guid userId, CancellationToken ct)
    {
        return Task.FromResult(new CartAbandonmentResult(0, 0, "No data available."));
    }
}

