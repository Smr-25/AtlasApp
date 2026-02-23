using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.DetectBurnoutRisk;

public class DetectBurnoutRiskQueryHandler(
    ILeaderAgentService agentService
) : IRequestHandler<DetectBurnoutRiskQuery, BurnoutRiskResult>
{
    public async Task<BurnoutRiskResult> Handle(DetectBurnoutRiskQuery request, CancellationToken cancellationToken)
    {
        return await agentService.DetectBurnoutRiskAsync(request.TeamId, cancellationToken);
    }
}

