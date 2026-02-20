using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Commands.UpdateSnippet;

public class UpdateSnippetCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateSnippetCommand, bool>
{
    public async Task<bool> Handle(UpdateSnippetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == request.SnippetId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Snippet", request.SnippetId);

        snippet.UpdateTitle(request.Title);
        snippet.UpdateCode(request.Code, request.Language);
        snippet.UpdateTags(request.Tags);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

