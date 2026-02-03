using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Docker.Commands.ControlContainer;

public class ControlContainerCommandHandler(IDockerService dockerService)
    : IRequestHandler<ControlContainerCommand, bool>
{
    public async Task<bool> Handle(ControlContainerCommand request, CancellationToken cancellationToken)
    {
        switch (request.Action)
        {
            case DockerAction.Start:
                await dockerService.StartContainerAsync(request.ContainerId, cancellationToken);
                break;
            case DockerAction.Stop:
                await dockerService.StopContainerAsync(request.ContainerId, cancellationToken);
                break;
            case DockerAction.Restart:
                await dockerService.RestartContainerAsync(request.ContainerId, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return true;
    }
}