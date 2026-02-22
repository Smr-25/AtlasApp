using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.KillIdleContainers;

public record KillIdleContainersCommand(int IdleMinutes = 60) : IRequest<string>;

