using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.PasteFromNotion;

public class PasteFromNotionCommandHandler(INotionService notionService)
    : IRequestHandler<PasteFromNotionCommand, PasteFromNotionResponse>
{
    public async Task<PasteFromNotionResponse> Handle(PasteFromNotionCommand request, CancellationToken cancellationToken)
    {
        var snippets = await notionService.FetchSnippetsFromNotionAsync(
            request.NotionDatabaseId,
            request.NotionAuthToken,
            1,
            cancellationToken
        );

        if (snippets.Count == 0)
            throw new NotFoundException("NotionSnippet", "No snippets found in Notion database.");

        var latest = snippets[0];

        return new PasteFromNotionResponse(
            latest.Title,
            latest.Code,
            latest.Language,
            latest.Url
        );
    }
}

