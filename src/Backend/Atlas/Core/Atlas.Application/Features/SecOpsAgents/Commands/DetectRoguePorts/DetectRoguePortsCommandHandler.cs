using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.DetectRoguePorts;

public class DetectRoguePortsCommandHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<DetectRoguePortsCommand, List<RoguePortInfo>>
{
    public async Task<List<RoguePortInfo>> Handle(DetectRoguePortsCommand request, CancellationToken cancellationToken)
    {
        return await agentService.DetectRoguePortsAsync(cancellationToken);
    }
}

