using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class NotificationService(
    IApplicationDbContext context,
    IAtlasHubService hubService) : INotificationService
{
    public async Task SendAsync(
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
        CancellationToken cancellationToken = default)
    {
        var notification = Notification.Create(
            userId, category, priority, title, body,
            actionType, actionPayloadJson, sourceEntity, sourceEntityId, workspaceId);

        await context.Notifications.AddAsync(notification, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await hubService.SendToUserAsync(userId, "NotificationReceived", new
        {
            notification.Id,
            Category = category.ToString(),
            Priority = priority.ToString(),
            Title = title,
            Body = body,
            ActionType = actionType
        });
    }

    public async Task SendToWorkspaceAsync(
        Guid workspaceId,
        NotificationCategory category,
        NotificationPriority priority,
        string title,
        string body,
        string? actionType = null,
        string? actionPayloadJson = null,
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var memberUserIds = await context.WorkspaceMembers
            .Where(wm => wm.WorkspaceId == workspaceId && !wm.IsDeleted)
            .Select(wm => wm.UserId)
            .ToListAsync(cancellationToken);

        foreach (var userId in memberUserIds)
        {
            if (excludeUserId.HasValue && userId == excludeUserId.Value) continue;

            await SendAsync(userId, category, priority, title, body,
                actionType, actionPayloadJson, workspaceId: workspaceId,
                cancellationToken: cancellationToken);
        }
    }
}
