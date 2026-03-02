using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Notifications.Commands.MarkAsRead;

public class MarkAsReadCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<MarkAsReadCommand>
{
    public async Task Handle(MarkAsReadCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var notification = await context.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == userId, ct)
            ?? throw new NotFoundException("Notification", request.NotificationId);

        notification.MarkAsRead();
        await context.SaveChangesAsync(ct);
    }
}

