using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectBrokenLinks;

public class DetectBrokenLinksQueryHandler(
    IMarketerAgentService agentService
) : IRequestHandler<DetectBrokenLinksQuery, List<BrokenLinkResult>>
{
    public async Task<List<BrokenLinkResult>> Handle(DetectBrokenLinksQuery request, CancellationToken cancellationToken)
    {
        return await agentService.DetectBrokenLinksAsync(request.BaseUrl, cancellationToken);
    }
}

