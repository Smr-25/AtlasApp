using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SquadArena.Queries.GetBountyBoard;

public class GetBountyBoardQueryHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<GetBountyBoardQuery, List<BountyBoardDto>>
{
    public async Task<List<BountyBoardDto>> Handle(GetBountyBoardQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.BountyBoards
            .Where(b => b.TeamId == request.TeamId)
            .OrderByDescending(b => b.RewardPoints)
            .Select(b => new BountyBoardDto(b.Id, b.Title, b.Description, b.RewardPoints, b.ClaimedByUserId, b.IsCompleted, b.JiraIssueKey))
            .ToListAsync(cancellationToken);
    }
}

