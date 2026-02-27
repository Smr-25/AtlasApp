using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.DetectBurnoutRisk;

public class DetectBurnoutRiskQueryHandler(
    ILeaderAgentService agentService,
    ICurrentUserService currentUser,
    IAtlasHubService hubService
) : IRequestHandler<DetectBurnoutRiskQuery, BurnoutRiskResult>
{
    public async Task<BurnoutRiskResult> Handle(DetectBurnoutRiskQuery request, CancellationToken cancellationToken)
    {
        var result = await agentService.DetectBurnoutRiskAsync(request.TeamId, cancellationToken);

        var atRiskMembers = result.Members.Where(m => m.RiskLevel is "High" or "Critical").ToList();
        if (atRiskMembers.Count > 0)
        {
            await hubService.SendAlertAsync(request.TeamId, "BurnoutRiskDetected", new
            {
                DetectedBy = currentUser.UserName,
                AtRiskMembers = atRiskMembers,
                Severity = atRiskMembers.Any(m => m.RiskLevel == "Critical") ? "Critical" : "High"
            }, cancellationToken);
        }

        return result;
    }
}

