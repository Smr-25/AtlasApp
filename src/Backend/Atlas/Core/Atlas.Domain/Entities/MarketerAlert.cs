using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class MarketerAlert : BaseEntity
{
    public MarketerAlertType Type { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ActionPayload { get; private set; }
    public bool IsRead { get; private set; }
    public bool IsActioned { get; private set; }
    public Guid UserId { get; private set; }

    private MarketerAlert() { }

    public static MarketerAlert Create(
        Guid userId,
        MarketerAlertType type,
        AlertSeverity severity,
        string title,
        string message,
        string? actionPayload = null)
    {
        return new MarketerAlert
        {
            UserId = userId,
            Type = type,
            Severity = severity,
            Title = title,
            Message = message,
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

