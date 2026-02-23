using Atlas.Application.Common.Interfaces;

namespace Atlas.Infrastructure.Adapters;

public class MailchimpAdapter(IHttpClientFactory httpClientFactory) : IMailchimpAdapter
{
    public Task<string> ResendCampaignAsync(string campaignId, string newSubject, CancellationToken ct)
        => Task.FromResult($"Campaign {campaignId} resent with subject: {newSubject}");

    public Task<List<MailchimpCampaign>> GetCampaignsAsync(CancellationToken ct)
        => Task.FromResult(new List<MailchimpCampaign>());

    public Task<MailchimpCampaignStats> GetCampaignStatsAsync(string campaignId, CancellationToken ct)
        => Task.FromResult(new MailchimpCampaignStats(campaignId, 0, 0, 0, 0, 0));
}

