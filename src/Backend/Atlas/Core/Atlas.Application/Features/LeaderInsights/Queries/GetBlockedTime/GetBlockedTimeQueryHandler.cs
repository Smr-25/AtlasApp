using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetBlockedTime;

public class GetBlockedTimeQueryHandler(
    ILeaderInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetBlockedTimeQuery, BlockedTimeResult>
{
    public async Task<BlockedTimeResult> Handle(GetBlockedTimeQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetBlockedTimeAsync(userId, request.TeamId, request.From, request.To, cancellationToken);
    }
}

