using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.RunScript;

public record RunScriptCommand(Guid Id) : IRequest<string>;