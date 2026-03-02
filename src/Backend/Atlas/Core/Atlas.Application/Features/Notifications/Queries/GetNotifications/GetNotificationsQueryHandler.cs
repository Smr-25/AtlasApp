using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Notifications.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Queries.GetNotifications;

public class GetNotificationsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var query = context.Notifications
            .Where(n => n.UserId == userId)
            .AsQueryable();

        if (request.Category.HasValue)
            query = query.Where(n => n.Category == request.Category.Value);

        if (request.UnreadOnly == true)
            query = query.Where(n => !n.IsRead);

        return await query
            .OrderByDescending(n => n.Priority)
            .ThenByDescending(n => n.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationDto(
                n.Id, n.Category, n.Priority, n.Title, n.Body,
                n.ActionType, n.ActionPayloadJson, n.SourceEntity, n.SourceEntityId,
                n.IsRead, n.ReadAt, n.WorkspaceId, n.CreatedAt))
            .ToListAsync(ct);
    }
}

