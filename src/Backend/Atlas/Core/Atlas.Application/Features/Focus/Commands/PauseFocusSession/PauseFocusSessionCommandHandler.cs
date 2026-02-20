using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Commands.PauseFocusSession;

public class PauseFocusSessionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<PauseFocusSessionCommand, bool>
{
    public async Task<bool> Handle(PauseFocusSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var session = await dbContext.FocusSessions
            .FirstOrDefaultAsync(fs => fs.Id == request.SessionId && fs.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("FocusSession", request.SessionId);

        if (session.Status != FocusSessionStatus.InProgress)
            throw new BadRequestException("Session is not in progress.");

        session.Pause();
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

