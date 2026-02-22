using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetDeploymentSuccessRate;

public class GetDeploymentSuccessRateQueryHandler(
    IInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetDeploymentSuccessRateQuery, DeploymentSuccessRateResult>
{
    public async Task<DeploymentSuccessRateResult> Handle(GetDeploymentSuccessRateQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var rate = await insightService.GetDeploymentSuccessRateAsync(userId, request.From, request.To, cancellationToken);
        return new DeploymentSuccessRateResult(rate, 0, 0);
    }
}

