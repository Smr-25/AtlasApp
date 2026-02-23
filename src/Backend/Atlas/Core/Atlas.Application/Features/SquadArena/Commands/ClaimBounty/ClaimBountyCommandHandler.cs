using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SquadArena.Commands.ClaimBounty;

public class ClaimBountyCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser
) : IRequestHandler<ClaimBountyCommand, Unit>
{
    public async Task<Unit> Handle(ClaimBountyCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var bounty = await dbContext.BountyBoards.FirstOrDefaultAsync(b => b.Id == request.BountyId, cancellationToken);
        if (bounty != null && bounty.ClaimedByUserId == null)
        {
            bounty.Claim(userId);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

