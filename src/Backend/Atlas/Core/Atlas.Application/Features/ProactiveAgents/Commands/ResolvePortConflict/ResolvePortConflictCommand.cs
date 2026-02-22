using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.ResolvePortConflict;

public record ResolvePortConflictCommand(int Port) : IRequest<string>;

