using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Snippets.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Snippets.Queries.GetSnippets;

public class GetSnippetsQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper)
    : IRequestHandler<GetSnippetsQuery, List<SnippetDto>>
{
    public async Task<List<SnippetDto>> Handle(GetSnippetsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        return await applicationDbContext.Snippets
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ProjectTo<SnippetDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}