using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.KillAllNodes;

public record KillAllNodesCommand() : IRequest<string>;

