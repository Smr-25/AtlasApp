using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(Guid NotificationId) : IRequest;

public class DeleteNotificationCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteNotificationCommand>
{
    public async Task Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == userId, ct)
            ?? throw new NotFoundException("Notification", request.NotificationId);

        notification.Delete();
        await context.SaveChangesAsync(ct);
    }
}

