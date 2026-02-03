using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.CreateScript;

public record CreateScriptCommand(
    string Name,
    string Command,
    string Arguments,
    string WorkingDirectory,
    string? Icon,
    string? Color
) : IRequest<Guid>;