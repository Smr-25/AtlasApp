using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SquadArenaEntry : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public int Points { get; private set; }
    public ArenaBadgeType BadgeType { get; private set; }
    public string? SprintId { get; private set; }
    public DateTime AwardedAt { get; private set; }
    public string? MetadataJson { get; private set; }

    private SquadArenaEntry() { }

    public static SquadArenaEntry Create(
        Guid teamId,
        Guid userId,
        ArenaBadgeType badgeType,
        int points,
        string? sprintId = null,
        string? metadataJson = null)
    {
        return new SquadArenaEntry
        {
            TeamId = teamId,
            UserId = userId,
            BadgeType = badgeType,
            Points = points,
            SprintId = sprintId,
            MetadataJson = metadataJson,
            AwardedAt = DateTime.UtcNow
        };
    }
}

