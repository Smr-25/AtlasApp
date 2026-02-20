using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Commands.DeleteSnippet;

public class DeleteSnippetCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<DeleteSnippetCommand, bool>
{
    public async Task<bool> Handle(DeleteSnippetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var snippet = await dbContext.Snippets
            .FirstOrDefaultAsync(s => s.Id == request.SnippetId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Snippet", request.SnippetId);

        dbContext.Snippets.Remove(snippet);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

