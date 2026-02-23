using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SquadArena.Commands.CompleteBounty;

public class CompleteBountyCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<CompleteBountyCommand, Unit>
{
    public async Task<Unit> Handle(CompleteBountyCommand request, CancellationToken cancellationToken)
    {
        var bounty = await dbContext.BountyBoards.FirstOrDefaultAsync(b => b.Id == request.BountyId, cancellationToken);
        if (bounty != null)
        {
            bounty.Complete();
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}

