using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class MetaAdsAdapter(IHttpClientFactory httpClientFactory) : IMetaAdsAdapter
{
    public Task<string> PauseCampaignAsync(string campaignId, CancellationToken ct)
        => Task.FromResult($"Campaign {campaignId} paused.");

    public Task<string> ResumeCampaignAsync(string campaignId, CancellationToken ct)
        => Task.FromResult($"Campaign {campaignId} resumed.");

    public Task<string> UpdateBudgetAsync(string campaignId, double newBudget, CancellationToken ct)
        => Task.FromResult($"Campaign {campaignId} budget updated to {newBudget}.");

    public Task<List<MetaAdCampaign>> GetCampaignsAsync(CancellationToken ct)
        => Task.FromResult(new List<MetaAdCampaign>());
}

