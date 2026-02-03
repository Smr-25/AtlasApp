using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.CreateSnippet;

public record CreateSnippetCommand(
    string Title, 
    string Code, 
    string Language, 
    List<string> Tags
) : IRequest<Guid>;