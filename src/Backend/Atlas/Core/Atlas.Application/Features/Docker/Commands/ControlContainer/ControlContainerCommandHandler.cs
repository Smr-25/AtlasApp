using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Docker.Commands.ControlContainer;

public class ControlContainerCommandHandler(IDockerAdapter dockerAdapter)
    : IRequestHandler<ControlContainerCommand, bool>
{
    public async Task<bool> Handle(ControlContainerCommand request, CancellationToken cancellationToken)
    {
        switch (request.Action)
        {
            case DockerAction.Start:
                await dockerAdapter.StartContainerAsync(request.ContainerId, cancellationToken);
                break;
            case DockerAction.Stop:
                await dockerAdapter.StopContainerAsync(request.ContainerId, cancellationToken);
                break;
            case DockerAction.Restart:
                await dockerAdapter.RestartContainerAsync(request.ContainerId, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return true;
    }
}