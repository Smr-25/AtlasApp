using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand(NotificationCategory? Category = null) : IRequest<int>;

public class MarkAllAsReadCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<MarkAllAsReadCommand, int>
{
    public async Task<int> Handle(MarkAllAsReadCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var query = context.Notifications.Where(n => n.UserId == userId && !n.IsRead);

        if (request.Category.HasValue)
            query = query.Where(n => n.Category == request.Category.Value);

        var notifications = await query.ToListAsync(ct);
        foreach (var n in notifications) n.MarkAsRead();

        await context.SaveChangesAsync(ct);
        return notifications.Count;
    }
}

