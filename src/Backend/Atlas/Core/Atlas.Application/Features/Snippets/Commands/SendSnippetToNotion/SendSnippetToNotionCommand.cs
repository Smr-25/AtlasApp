using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.SendSnippetToNotion;

public record SendSnippetToNotionCommand(
    Guid SnippetId,
    string NotionDatabaseId,
    string NotionAuthToken
) : IRequest<string>;

