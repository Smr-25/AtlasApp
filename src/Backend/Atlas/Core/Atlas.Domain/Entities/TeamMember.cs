using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid TeamId { get; private set; }
    public Guid UserId { get; private set; }
    public TeamMemberRole Role { get; private set; }
    public DateTime JoinedAt { get; private set; }

    private TeamMember() { }

    public static TeamMember Create(Guid teamId, Guid userId, TeamMemberRole role)
    {
        return new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            Role = role,
            JoinedAt = DateTime.UtcNow
        };
    }

    public void PromoteToManager()
    {
        Role = TeamMemberRole.Manager;
        SetModified();
    }

    public void DemoteToMember()
    {
        Role = TeamMemberRole.Member;
        SetModified();
    }

    public void Remove()
    {
        SetDelete();
    }
}

