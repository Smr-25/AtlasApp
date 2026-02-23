using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Commands.KillZombieProcesses;

public class KillZombieProcessesCommandHandler(
    ISecOpsAgentService agentService
) : IRequestHandler<KillZombieProcessesCommand, List<ZombieProcessInfo>>
{
    public async Task<List<ZombieProcessInfo>> Handle(KillZombieProcessesCommand request, CancellationToken cancellationToken)
    {
        return await agentService.KillZombieProcessesAsync(cancellationToken);
    }
}

