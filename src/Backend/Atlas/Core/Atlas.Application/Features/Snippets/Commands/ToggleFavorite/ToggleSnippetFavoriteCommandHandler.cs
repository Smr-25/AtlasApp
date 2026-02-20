using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Commands.ToggleFavorite;

public class ToggleSnippetFavoriteCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<ToggleSnippetFavoriteCommand, bool>
{
    public async Task<bool> Handle(ToggleSnippetFavoriteCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == request.SnippetId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Snippet", request.SnippetId);

        snippet.ToggleFavorite();
        await dbContext.SaveChangesAsync(cancellationToken);

        return snippet.IsFavorite;
    }
}

