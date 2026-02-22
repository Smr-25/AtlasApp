using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.KillIdleContainers;

public class KillIdleContainersCommandHandler(
    IDockerAdapter dockerAdapter
) : IRequestHandler<KillIdleContainersCommand, string>
{
    public async Task<string> Handle(KillIdleContainersCommand request, CancellationToken cancellationToken)
    {
        var containers = await dockerAdapter.GetContainersAsync(cancellationToken);
        var stopped = 0;

        foreach (var container in containers.Where(c => c.State == "running"))
        {
            await dockerAdapter.StopContainerAsync(container.Id, cancellationToken);
            stopped++;
        }

        return $"Stopped {stopped} idle container(s).";
    }
}

