using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.PredictBottleneck;

public class PredictBottleneckQueryHandler(
    ILeaderAgentService agentService
) : IRequestHandler<PredictBottleneckQuery, BottleneckResult>
{
    public async Task<BottleneckResult> Handle(PredictBottleneckQuery request, CancellationToken cancellationToken)
    {
        return await agentService.PredictBottleneckAsync(request.TeamId, cancellationToken);
    }
}

