using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Snippets.Commands.CreateSnippet;

public class CreateSnippetCommandHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<CreateSnippetCommand, Guid>
{
    public async Task<Guid> Handle(CreateSnippetCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var snippet = Snippet.Create(
            title: request.Title,
            code: request.Code,
            language: request.Language,
            tags: request.Tags,
            userId: userId
        );

        await applicationDbContext.Snippets.AddAsync(snippet, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return snippet.Id;
    }
}