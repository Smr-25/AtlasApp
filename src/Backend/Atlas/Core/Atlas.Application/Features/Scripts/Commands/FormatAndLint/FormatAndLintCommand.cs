using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.FormatAndLint;

public record FormatAndLintCommand(string ProjectPath) : IRequest<string>;

