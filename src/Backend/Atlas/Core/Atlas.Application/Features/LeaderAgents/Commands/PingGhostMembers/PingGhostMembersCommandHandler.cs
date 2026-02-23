using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Commands.PingGhostMembers;

public class PingGhostMembersCommandHandler(
    ILeaderAgentService agentService
) : IRequestHandler<PingGhostMembersCommand, GhostMemberResult>
{
    public async Task<GhostMemberResult> Handle(PingGhostMembersCommand request, CancellationToken cancellationToken)
    {
        return await agentService.PingGhostMembersAsync(request.TeamId, cancellationToken);
    }
}

