using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Commands.InviteMember;

public class InviteMemberCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<InviteMemberCommand, bool>
{
    public async Task<bool> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.OwnerUserId != userId)
            throw new ForbiddenException("Only the team owner can invite members.");

        team.AddMember(request.UserId);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

