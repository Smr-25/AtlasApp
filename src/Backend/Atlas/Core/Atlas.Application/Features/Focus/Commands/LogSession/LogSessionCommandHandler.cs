using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Focus.Commands.LogSession;

public class LogSessionCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<LogSessionCommand, Guid>
{
    public async Task<Guid> Handle(LogSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        if (userId == null)
            throw new UnauthorizedAccessException("User is not authenticated.");

        var parsedUserId = Guid.Parse(userId);

        var session = FocusSession.Create(
            request.DurationMinutes,
            request.Tag,
            parsedUserId,
            request.SessionType,
            request.BreakDurationMinutes,
            request.WorkspaceId
        );
        await applicationDbContext.FocusSessions.AddAsync(session, cancellationToken);

        var activity = UserActivity.Create(
            parsedUserId,
            "FocusSessionStarted",
            $"Started {request.SessionType} session ({request.DurationMinutes} min)",
            request.WorkspaceId,
            session.Id
        );
        await applicationDbContext.UserActivities.AddAsync(activity, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}