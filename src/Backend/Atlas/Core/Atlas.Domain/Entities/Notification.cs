using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;

namespace Atlas.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; private set; }
    public NotificationCategory Category { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string? ActionType { get; private set; }
    public string? ActionPayloadJson { get; private set; }
    public string? SourceEntity { get; private set; }
    public Guid? SourceEntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public Guid? WorkspaceId { get; private set; }

    private Notification() { }

    public static Notification Create(
        Guid userId,
        NotificationCategory category,
        NotificationPriority priority,
        string title,
        string body,
        string? actionType = null,
        string? actionPayloadJson = null,
        string? sourceEntity = null,
        Guid? sourceEntityId = null,
        Guid? workspaceId = null)
    {
        return new Notification
        {
            UserId = userId,
            Category = category,
            Priority = priority,
            Title = title,
            Body = body,
            ActionType = actionType,
            ActionPayloadJson = actionPayloadJson,
            SourceEntity = sourceEntity,
            SourceEntityId = sourceEntityId,
            WorkspaceId = workspaceId
        };
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
            SetModified();
        }
    }

    public void Delete() => SetDelete();
}

