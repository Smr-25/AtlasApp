using MediatR;

namespace Atlas.Application.Features.Docker.Commands.ControlContainer;

public record ControlContainerCommand(string ContainerId, DockerAction Action) : IRequest<bool>;

public enum DockerAction
{
    Start,
    Stop,
    Restart
}