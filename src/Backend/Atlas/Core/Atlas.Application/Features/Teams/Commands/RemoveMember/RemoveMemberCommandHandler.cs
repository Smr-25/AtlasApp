using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Commands.RemoveMember;

public class RemoveMemberCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<RemoveMemberCommand, bool>
{
    public async Task<bool> Handle(RemoveMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.OwnerUserId != userId)
            throw new ForbiddenException("Only the team owner can remove members.");

        team.RemoveMember(request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

