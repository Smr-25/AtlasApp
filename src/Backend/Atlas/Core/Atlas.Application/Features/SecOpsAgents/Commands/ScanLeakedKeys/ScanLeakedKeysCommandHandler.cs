using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.ScanLeakedKeys;

public class ScanLeakedKeysCommandHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<ScanLeakedKeysCommand, List<LeakedKeyInfo>>
{
    public async Task<List<LeakedKeyInfo>> Handle(ScanLeakedKeysCommand request, CancellationToken cancellationToken)
    {
        return await agentService.ScanLeakedKeysAsync(request.Content, cancellationToken);
    }
}

