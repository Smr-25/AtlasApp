namespace Atlas.Application.Common.Interfaces;

public interface IMetaAdsAdapter
{
    Task<string> PauseCampaignAsync(string campaignId, CancellationToken ct);
    Task<string> ResumeCampaignAsync(string campaignId, CancellationToken ct);
    Task<string> UpdateBudgetAsync(string campaignId, double newBudget, CancellationToken ct);
    Task<List<MetaAdCampaign>> GetCampaignsAsync(CancellationToken ct);
}

public record MetaAdCampaign(string Id, string Name, string Status, double DailyBudget, double Spend, int Impressions);

