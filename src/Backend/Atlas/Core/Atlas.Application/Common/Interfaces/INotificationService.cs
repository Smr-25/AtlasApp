using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendAsync(
        Guid userId,
        NotificationCategory category,
        NotificationPriority priority,
        string title,
        string body,
        string? actionType = null,
        string? actionPayloadJson = null,
        string? sourceEntity = null,
        Guid? sourceEntityId = null,
        Guid? workspaceId = null,
        CancellationToken cancellationToken = default);

    Task SendToWorkspaceAsync(
        Guid workspaceId,
        NotificationCategory category,
        NotificationPriority priority,
        string title,
        string body,
        string? actionType = null,
        string? actionPayloadJson = null,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);
}

