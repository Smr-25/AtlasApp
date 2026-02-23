using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.CatchUnassignedBugs;

public class CatchUnassignedBugsQueryHandler(
    ILeaderAgentService agentService
) : IRequestHandler<CatchUnassignedBugsQuery, UnassignedBugResult>
{
    public async Task<UnassignedBugResult> Handle(CatchUnassignedBugsQuery request, CancellationToken cancellationToken)
    {
        return await agentService.CatchUnassignedBugsAsync(request.TeamId, cancellationToken);
    }
}

