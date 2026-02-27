using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.SecOpsAgents.Queries.DetectSuspiciousTraffic;

public class DetectSuspiciousTrafficQueryHandler(
    ISecOpsAgentService agentService,
    ICurrentUserService currentUser,
    IApplicationDbContext dbContext,
    IAtlasHubService hubService
) : IRequestHandler<DetectSuspiciousTrafficQuery, TrafficAnalysisResult>
{
    public async Task<TrafficAnalysisResult> Handle(DetectSuspiciousTrafficQuery request, CancellationToken cancellationToken)
    {
        var result = await agentService.DetectSuspiciousTrafficAsync(request.TargetUrl, cancellationToken);

        if (result.IsSuspicious)
        {
            var userId = Guid.Parse(currentUser.UserId!);
            var member = await dbContext.TeamMembers.FirstOrDefaultAsync(tm => tm.UserId == userId, cancellationToken);
            if (member != null)
            {
                await hubService.SendAlertAsync(member.TeamId, "SuspiciousTrafficDetected", new
                {
                    DetectedBy = currentUser.UserName,
                    TargetUrl = request.TargetUrl,
                    Analysis = result,
                    Severity = "High"
                }, cancellationToken);
            }
        }

        return result;
    }
}

