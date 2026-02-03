using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Snippets.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Queries.GetSnippets;

public class GetSnippetsQueryHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetSnippetsQuery, List<SnippetDto>>
{
    public async Task<List<SnippetDto>> Handle(GetSnippetsQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUserService.UserId ?? Guid.Empty.ToString());

        var entities = await applicationDbContext.Snippets
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
            
        return entities.Select(s => new SnippetDto(
            s.Id,
            s.Title,
            s.Code,
            s.Language,
            string.IsNullOrEmpty(s.Tags) ? Array.Empty<string>() : s.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries),
            s.IsFavorite,
            s.CreatedAt
        )).ToList();
    }
}