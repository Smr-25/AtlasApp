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

        var session = FocusSession.Create(
            request.DurationMinutes,
            request.Tag,
            Guid.Parse(userId)
        );
        await applicationDbContext.FocusSessions.AddAsync(session, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}