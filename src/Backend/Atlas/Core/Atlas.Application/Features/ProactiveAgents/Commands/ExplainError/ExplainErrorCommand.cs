using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Commands.ExplainError;

public record ExplainErrorCommand(string ErrorMessage, string? StackTrace) : IRequest<string>;

