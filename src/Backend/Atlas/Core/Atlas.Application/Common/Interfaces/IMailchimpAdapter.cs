namespace Atlas.Application.Common.Interfaces;

public interface IMailchimpAdapter
{
    Task<string> ResendCampaignAsync(string campaignId, string newSubject, CancellationToken ct);
    Task<List<MailchimpCampaign>> GetCampaignsAsync(CancellationToken ct);
    Task<MailchimpCampaignStats> GetCampaignStatsAsync(string campaignId, CancellationToken ct);
}

public record MailchimpCampaign(string Id, string Title, string Status, int Recipients, DateTime? SendTime);
public record MailchimpCampaignStats(string CampaignId, int Sent, int Opens, int Clicks, double OpenRate, double ClickRate);

