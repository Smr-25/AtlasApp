using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.CheckVpnDrop;

public class CheckVpnDropQueryHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<CheckVpnDropQuery, VpnStatusResult>
{
    public async Task<VpnStatusResult> Handle(CheckVpnDropQuery request, CancellationToken cancellationToken)
    {
        return await agentService.CheckVpnDropAsync(cancellationToken);
    }
}

