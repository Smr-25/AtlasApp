using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Events;

namespace Atlas.Domain.Entities;

public class FocusSession : BaseEntity
{
    public int DurationMinutes { get; private set; } 
    public int BreakDurationMinutes { get; private set; }
    public string Tag { get; private set; } = "Work"; 
    public FocusSessionType SessionType { get; private set; }
    public FocusSessionStatus Status { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? PausedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; } 
    public Guid UserId { get; private set; }
    public Guid? WorkspaceId { get; private set; }

    private FocusSession() { }

    public static FocusSession Create(int durationMinutes, string tag, Guid userId,
        FocusSessionType sessionType = FocusSessionType.Pomodoro,
        int breakDurationMinutes = 5, Guid? workspaceId = null)
    {
        return new FocusSession
        {
            Id = Guid.NewGuid(),
            DurationMinutes = durationMinutes,
            BreakDurationMinutes = breakDurationMinutes,
            Tag = tag,
            SessionType = sessionType,
            Status = FocusSessionStatus.InProgress,
            StartedAt = DateTime.UtcNow,
            UserId = userId,
            WorkspaceId = workspaceId
        };
    }

    public void Pause()
    {
        Status = FocusSessionStatus.Paused;
        PausedAt = DateTime.UtcNow;
        SetModified();
    }

    public void Resume()
    {
        Status = FocusSessionStatus.InProgress;
        PausedAt = null;
        SetModified();
    }

    public void Complete()
    {
        Status = FocusSessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new FocusSessionCompletedEvent(Id, UserId, DurationMinutes, WorkspaceId));
        SetModified();
    }

    public void Interrupt()
    {
        Status = FocusSessionStatus.Interrupted;
        CompletedAt = DateTime.UtcNow;
        SetModified();
    }
}