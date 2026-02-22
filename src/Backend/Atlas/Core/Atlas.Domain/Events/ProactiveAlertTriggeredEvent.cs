using Atlas.Domain.Enums;

namespace Atlas.Domain.Events;

public class ProactiveAlertTriggeredEvent : DomainEventBase
{
    public Guid AlertId { get; }
    public Guid UserId { get; }
    public AlertType AlertType { get; }
    public AlertSeverity Severity { get; }

    public ProactiveAlertTriggeredEvent(Guid alertId, Guid userId, AlertType alertType, AlertSeverity severity)
    {
        AlertId = alertId;
        UserId = userId;
        AlertType = alertType;
        Severity = severity;
    }
}

