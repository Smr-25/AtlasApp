using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Commands.SendSnippetToNotion;

public class SendSnippetToNotionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    INotionService notionService)
    : IRequestHandler<SendSnippetToNotionCommand, string>
{
    public async Task<string> Handle(SendSnippetToNotionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == request.SnippetId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Snippet", request.SnippetId);

        // Send to Notion - does NOT take space in our DB!
        var notionPageId = await notionService.SendSnippetToNotionAsync(
            snippet.Title,
            snippet.Code,
            snippet.Language,
            request.NotionDatabaseId,
            request.NotionAuthToken,
            cancellationToken
        );

        return notionPageId;
    }
}

