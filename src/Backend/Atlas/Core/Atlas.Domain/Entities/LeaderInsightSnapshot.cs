using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class LeaderInsightSnapshot : BaseEntity
{
    public LeaderInsightType Type { get; private set; }
    public string MetricKey { get; private set; } = null!;
    public double Value { get; private set; }
    public string? Unit { get; private set; }
    public string? MetadataJson { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? TeamId { get; private set; }

    private LeaderInsightSnapshot() { }

    public static LeaderInsightSnapshot Create(
        Guid userId,
        LeaderInsightType type,
        string metricKey,
        double value,
        string? unit = null,
        Guid? teamId = null,
        string? metadataJson = null)
    {
        return new LeaderInsightSnapshot
        {
            UserId = userId,
            Type = type,
            MetricKey = metricKey,
            Value = value,
            Unit = unit,
            TeamId = teamId,
            MetadataJson = metadataJson,
            RecordedAt = DateTime.UtcNow
        };
    }
}

