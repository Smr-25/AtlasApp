using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class SquadRadarEntry : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public SquadMemberStatus OnlineStatus { get; private set; }
    public string? CurrentToolIcon { get; private set; }
    public string? CurrentFocus { get; private set; }
    public DateTime LastActiveAt { get; private set; }
    public string? ActiveIntegrationsJson { get; private set; }
    public int? MeetingMinutesLeft { get; private set; }

    private SquadRadarEntry() { }

    public static SquadRadarEntry Create(Guid teamId, Guid userId)
    {
        return new SquadRadarEntry
        {
            TeamId = teamId,
            UserId = userId,
            OnlineStatus = SquadMemberStatus.Offline,
            LastActiveAt = DateTime.UtcNow
        };
    }

    public void UpdatePresence(SquadMemberStatus status, string? toolIcon = null, string? focus = null, int? meetingMinutesLeft = null)
    {
        OnlineStatus = status;
        CurrentToolIcon = toolIcon;
        CurrentFocus = focus;
        MeetingMinutesLeft = meetingMinutesLeft;
        LastActiveAt = DateTime.UtcNow;
        SetModified();
    }

    public void SetActiveIntegrations(string integrationsJson)
    {
        ActiveIntegrationsJson = integrationsJson;
        SetModified();
    }
}

