using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SquadArena.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<GetLeaderboardQuery, List<LeaderboardEntry>>
{
    public async Task<List<LeaderboardEntry>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var entries = await dbContext.SquadArenaEntries
            .Where(e => e.TeamId == request.TeamId)
            .GroupBy(e => e.UserId)
            .Select(g => new LeaderboardEntry(
                g.Key,
                g.Sum(e => e.Points),
                g.Select(e => e.BadgeType.ToString()).Distinct().ToList()
            ))
            .OrderByDescending(e => e.TotalPoints)
            .ToListAsync(cancellationToken);

        return entries;
    }
}

