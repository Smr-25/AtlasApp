namespace Atlas.Application.Common.Interfaces;

public interface IMarketerAgentService
{
    Task<BudgetBleedResult> DetectBudgetBleedAsync(Guid userId, CancellationToken ct);
    Task<List<BrokenLinkResult>> DetectBrokenLinksAsync(string baseUrl, CancellationToken ct);
    Task<List<TrendResult>> GetViralTrendsAsync(string industry, CancellationToken ct);
    Task<List<CompetitorPriceResult>> DetectCompetitorPriceDropAsync(string competitorUrl, CancellationToken ct);
    Task<string> ResendLowOpenRateAsync(string campaignId, string newSubject, CancellationToken ct);
    Task<string> AppendUtmAsync(string url, string source, string medium, string campaign, CancellationToken ct);
    Task<CartAbandonmentResult> DetectCartAbandonmentAsync(Guid userId, CancellationToken ct);
}

public record BudgetBleedResult(bool HasBleed, List<BleedingCampaign> Campaigns);
public record BleedingCampaign(string CampaignId, string Name, double Spend, double Revenue, double Roas);
public record BrokenLinkResult(string Url, int StatusCode, string? ErrorMessage);
public record TrendResult(string Hashtag, string Platform, int Volume, string Sentiment);
public record CompetitorPriceResult(string ProductName, double OldPrice, double NewPrice, double DiscountPercent);
public record CartAbandonmentResult(int AbandonedCount, double AbandonmentRate, string Recommendation);

