using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.DetectSuspiciousTraffic;

public class DetectSuspiciousTrafficQueryHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<DetectSuspiciousTrafficQuery, TrafficAnalysisResult>
{
    public async Task<TrafficAnalysisResult> Handle(DetectSuspiciousTrafficQuery request, CancellationToken cancellationToken)
    {
        return await agentService.DetectSuspiciousTrafficAsync(request.TargetUrl, cancellationToken);
    }
}

