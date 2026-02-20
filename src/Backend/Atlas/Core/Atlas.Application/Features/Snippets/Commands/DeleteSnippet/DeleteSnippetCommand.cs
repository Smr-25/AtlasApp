using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.DeleteSnippet;

public record DeleteSnippetCommand(Guid SnippetId) : IRequest<bool>;

