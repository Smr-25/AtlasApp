using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class LeaderAlert : BaseEntity
{
    public LeaderAlertType Type { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ActionPayload { get; private set; }
    public bool IsRead { get; private set; }
    public bool IsActioned { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? TeamId { get; private set; }

    private LeaderAlert() { }

    public static LeaderAlert Create(
        Guid userId,
        LeaderAlertType type,
        AlertSeverity severity,
        string title,
        string message,
        Guid? teamId = null,
        string? actionPayload = null)
    {
        return new LeaderAlert
        {
            UserId = userId,
            Type = type,
            Severity = severity,
            Title = title,
            Message = message,
            TeamId = teamId,
            ActionPayload = actionPayload,
            IsRead = false,
            IsActioned = false
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        SetModified();
    }

    public void MarkAsActioned()
    {
        IsActioned = true;
        IsRead = true;
        SetModified();
    }
}

