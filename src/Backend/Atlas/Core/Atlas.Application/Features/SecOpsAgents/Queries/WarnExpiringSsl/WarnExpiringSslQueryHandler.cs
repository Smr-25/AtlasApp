using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.WarnExpiringSsl;

public class WarnExpiringSslQueryHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<WarnExpiringSslQuery, List<ExpiringSslInfo>>
{
    public async Task<List<ExpiringSslInfo>> Handle(WarnExpiringSslQuery request, CancellationToken cancellationToken)
    {
        return await agentService.WarnExpiringSslAsync(request.Domains, cancellationToken);
    }
}

