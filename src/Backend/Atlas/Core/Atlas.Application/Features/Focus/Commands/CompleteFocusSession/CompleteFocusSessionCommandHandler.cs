using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Commands.CompleteFocusSession;

public class CompleteFocusSessionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<CompleteFocusSessionCommand, bool>
{
    public async Task<bool> Handle(CompleteFocusSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var session = await dbContext.FocusSessions
            .FirstOrDefaultAsync(fs => fs.Id == request.SessionId && fs.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("FocusSession", request.SessionId);

        if (session.Status != FocusSessionStatus.InProgress && session.Status != FocusSessionStatus.Paused)
            throw new BadRequestException("Session is not in a completable state.");

        session.Complete();

        var activity = UserActivity.Create(
            userId,
            "FocusSessionCompleted",
            $"Completed {session.SessionType} session ({session.DurationMinutes} min)",
            session.WorkspaceId,
            session.Id
        );
        await dbContext.UserActivities.AddAsync(activity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
