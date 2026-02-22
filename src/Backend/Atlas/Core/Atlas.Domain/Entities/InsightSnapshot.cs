using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class InsightSnapshot : BaseEntity
{
    public InsightType Type { get; private set; }
    public string MetricKey { get; private set; } = null!;
    public double Value { get; private set; }
    public string? Unit { get; private set; }
    public string? MetadataJson { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public Guid UserId { get; private set; }

    private InsightSnapshot() { }

    public static InsightSnapshot Create(
        Guid userId,
        InsightType type,
        string metricKey,
        double value,
        string? unit = null,
        string? metadataJson = null)
    {
        return new InsightSnapshot
        {
            UserId = userId,
            Type = type,
            MetricKey = metricKey,
            Value = value,
            Unit = unit,
            MetadataJson = metadataJson,
            RecordedAt = DateTime.UtcNow
        };
    }
}

