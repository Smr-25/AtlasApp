using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.DetectRoguePorts;

public class DetectRoguePortsCommandHandler(
    ISecOpsAgentService agentService,
    ICurrentUserService currentUser,
    IApplicationDbContext dbContext,
    IAtlasHubService hubService
) : IRequestHandler<DetectRoguePortsCommand, List<RoguePortInfo>>
{
    public async Task<List<RoguePortInfo>> Handle(DetectRoguePortsCommand request, CancellationToken cancellationToken)
    {
        var result = await agentService.DetectRoguePortsAsync(cancellationToken);

        if (result.Count > 0)
        {
            var userId = Guid.Parse(currentUser.UserId!);
            var teamMember = await GetTeamIdAsync(userId, cancellationToken);
            if (teamMember.HasValue)
            {
                await hubService.SendAlertAsync(teamMember.Value, "RoguePortDetected", new
                {
                    DetectedBy = currentUser.UserName,
                    RoguePorts = result,
                    Severity = "Critical"
                }, cancellationToken);
            }
        }

        return result;
    }

    private async Task<Guid?> GetTeamIdAsync(Guid userId, CancellationToken ct)
    {
        var member = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(dbContext.TeamMembers, tm => tm.UserId == userId, ct);
        return member?.TeamId;
    }
}

