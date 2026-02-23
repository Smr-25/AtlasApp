using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetCostPerFeature;

public class GetCostPerFeatureQueryHandler(
    ILeaderInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetCostPerFeatureQuery, CostPerFeatureResult>
{
    public async Task<CostPerFeatureResult> Handle(GetCostPerFeatureQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetCostPerFeatureAsync(userId, request.TeamId, request.From, request.To, cancellationToken);
    }
}

