namespace Atlas.Domain.Events;

public class FocusSessionCompletedEvent : DomainEventBase
{
    public Guid SessionId { get; }
    public Guid UserId { get; }
    public int DurationMinutes { get; }
    public Guid? WorkspaceId { get; }

    public FocusSessionCompletedEvent(Guid sessionId, Guid userId, int durationMinutes, Guid? workspaceId)
    {
        SessionId = sessionId;
        UserId = userId;
        DurationMinutes = durationMinutes;
        WorkspaceId = workspaceId;
    }
}

