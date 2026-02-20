using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.UpdateSnippet;

public record UpdateSnippetCommand(Guid SnippetId, string Title, string Code, string Language, List<string> Tags) : IRequest<bool>;

