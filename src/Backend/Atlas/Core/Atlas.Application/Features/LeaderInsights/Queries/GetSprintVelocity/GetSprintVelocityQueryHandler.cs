using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetSprintVelocity;

public class GetSprintVelocityQueryHandler(
    ILeaderInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetSprintVelocityQuery, SprintVelocityResult>
{
    public async Task<SprintVelocityResult> Handle(GetSprintVelocityQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetSprintVelocityAsync(userId, request.TeamId, request.From, request.To, cancellationToken);
    }
}

