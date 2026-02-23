using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.CreateBounty;

public class CreateBountyCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<CreateBountyCommand, Guid>
{
    public async Task<Guid> Handle(CreateBountyCommand request, CancellationToken cancellationToken)
    {
        var bounty = BountyBoard.Create(request.TeamId, request.Title, request.RewardPoints, request.Description, request.JiraIssueKey);
        dbContext.BountyBoards.Add(bounty);
        await dbContext.SaveChangesAsync(cancellationToken);
        return bounty.Id;
    }
}

