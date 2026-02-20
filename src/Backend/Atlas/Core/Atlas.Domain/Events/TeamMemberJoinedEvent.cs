namespace Atlas.Domain.Events;

public class TeamMemberJoinedEvent : DomainEventBase
{
    public Guid TeamId { get; }
    public Guid UserId { get; }

    public TeamMemberJoinedEvent(Guid teamId, Guid userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}

