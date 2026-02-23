using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.CheckMilestoneCelebration;

public class CheckMilestoneCelebrationQueryHandler(
    ILeaderAgentService agentService
) : IRequestHandler<CheckMilestoneCelebrationQuery, MilestoneCelebrationResult>
{
    public async Task<MilestoneCelebrationResult> Handle(CheckMilestoneCelebrationQuery request, CancellationToken cancellationToken)
    {
        return await agentService.CheckMilestoneCelebrationAsync(request.TeamId, cancellationToken);
    }
}

