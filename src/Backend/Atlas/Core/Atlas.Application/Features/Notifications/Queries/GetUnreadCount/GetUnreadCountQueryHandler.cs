using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Notifications.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Queries.GetUnreadCount;

public class GetUnreadCountQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetUnreadCountQuery, NotificationCountDto>
{
    public async Task<NotificationCountDto> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var unread = await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .GroupBy(n => n.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return new NotificationCountDto(
            Total: unread.Sum(x => x.Count),
            AlertsSecOps: unread.FirstOrDefault(x => x.Category == NotificationCategory.AlertsSecOps)?.Count ?? 0,
            ApprovalsGit: unread.FirstOrDefault(x => x.Category == NotificationCategory.ApprovalsGit)?.Count ?? 0,
            MentionsSocial: unread.FirstOrDefault(x => x.Category == NotificationCategory.MentionsSocial)?.Count ?? 0,
            SystemInsights: unread.FirstOrDefault(x => x.Category == NotificationCategory.SystemInsights)?.Count ?? 0
        );
    }
}

