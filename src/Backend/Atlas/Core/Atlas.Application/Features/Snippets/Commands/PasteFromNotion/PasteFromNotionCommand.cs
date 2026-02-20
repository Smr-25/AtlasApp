using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.PasteFromNotion;

public record PasteFromNotionCommand(
    string NotionDatabaseId,
    string NotionAuthToken
) : IRequest<PasteFromNotionResponse>;

public record PasteFromNotionResponse(
    string Title,
    string Code,
    string Language,
    string NotionUrl
);

