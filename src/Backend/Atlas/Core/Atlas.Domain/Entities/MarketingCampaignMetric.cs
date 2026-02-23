using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class MarketingCampaignMetric : BaseEntity
{
    public string CampaignId { get; private set; } = null!;
    public string Platform { get; private set; } = null!;
    public double Spend { get; private set; }
    public double Revenue { get; private set; }
    public int Impressions { get; private set; }
    public int Clicks { get; private set; }
    public int Conversions { get; private set; }
    public string? MetadataJson { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public Guid UserId { get; private set; }

    private MarketingCampaignMetric() { }

    public static MarketingCampaignMetric Create(
        Guid userId,
        string campaignId,
        string platform,
        double spend,
        double revenue,
        int impressions,
        int clicks,
        int conversions,
        string? metadataJson = null)
    {
        return new MarketingCampaignMetric
        {
            UserId = userId,
            CampaignId = campaignId,
            Platform = platform,
            Spend = spend,
            Revenue = revenue,
            Impressions = impressions,
            Clicks = clicks,
            Conversions = conversions,
            MetadataJson = metadataJson,
            RecordedAt = DateTime.UtcNow
        };
    }
}

