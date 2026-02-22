using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class DesignAlert : BaseEntity
{
    public DesignAlertType Type { get; private set; }
    public AlertSeverity Severity { get; private set; }
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ActionPayload { get; private set; }
    public bool IsRead { get; private set; }
    public bool IsActioned { get; private set; }
    public Guid UserId { get; private set; }

    private DesignAlert() { }

    public static DesignAlert Create(
        Guid userId,
        DesignAlertType type,
        AlertSeverity severity,
        string title,
        string message,
        string? actionPayload = null)
    {
        return new DesignAlert
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

