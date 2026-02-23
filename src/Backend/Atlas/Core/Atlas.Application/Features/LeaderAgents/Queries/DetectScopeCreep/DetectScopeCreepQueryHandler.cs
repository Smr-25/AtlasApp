using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.DetectScopeCreep;

public class DetectScopeCreepQueryHandler(
    ILeaderAgentService agentService
) : IRequestHandler<DetectScopeCreepQuery, ScopeCreepResult>
{
    public async Task<ScopeCreepResult> Handle(DetectScopeCreepQuery request, CancellationToken cancellationToken)
    {
        return await agentService.DetectScopeCreepAsync(request.TeamId, request.SprintId, cancellationToken);
    }
}

