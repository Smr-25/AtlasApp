using MediatR;

namespace Atlas.Application.Features.SquadArena.Queries.GetLeaderboard;

public record GetLeaderboardQuery(Guid TeamId) : IRequest<List<LeaderboardEntry>>;

public record LeaderboardEntry(Guid UserId, int TotalPoints, List<string> Badges);

