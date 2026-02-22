using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.SpinEnvironment;

public record SpinEnvironmentCommand(string DockerComposePath, string? ProjectPath) : IRequest<string>;

