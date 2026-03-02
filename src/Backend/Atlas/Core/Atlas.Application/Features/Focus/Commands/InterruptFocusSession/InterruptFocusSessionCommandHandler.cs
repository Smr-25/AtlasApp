using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Commands.InterruptFocusSession;

public class InterruptFocusSessionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<InterruptFocusSessionCommand, bool>
{
    public async Task<bool> Handle(InterruptFocusSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var session = await dbContext.FocusSessions
            .FirstOrDefaultAsync(fs => fs.Id == request.SessionId && fs.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("FocusSession", request.SessionId);

        if (session.Status is FocusSessionStatus.Completed or FocusSessionStatus.Interrupted)
            throw new BadRequestException("Session is already finished.");

        session.Interrupt();

        var activity = UserActivity.Create(
            userId,
            "FocusSessionInterrupted",
            $"Interrupted {session.SessionType} session",
            session.WorkspaceId,
            session.Id
        );
        await dbContext.UserActivities.AddAsync(activity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
