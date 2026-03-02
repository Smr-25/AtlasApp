using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Commands.ExecuteAction;

public record ExecuteNotificationActionCommand(Guid NotificationId) : IRequest<string>;

public class ExecuteNotificationActionCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<ExecuteNotificationActionCommand, string>
{
    public async Task<string> Handle(ExecuteNotificationActionCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == userId, ct)
            ?? throw new NotFoundException("Notification", request.NotificationId);

        if (string.IsNullOrEmpty(notification.ActionType))
            throw new BadRequestException("This notification has no actionable item.");

        notification.MarkAsRead();
        await context.SaveChangesAsync(ct);

        return notification.ActionPayloadJson ?? "{}";
    }
}
