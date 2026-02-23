using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.ResourceHub.Queries.GetResources;

public class GetResourcesQueryHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<GetResourcesQuery, List<SharedResourceDto>>
{
    public async Task<List<SharedResourceDto>> Handle(GetResourcesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.SharedResources.Where(r => r.TeamId == request.TeamId);

        if (request.CategoryFilter.HasValue)
            query = query.Where(r => r.Category == request.CategoryFilter.Value);

        return await query
            .OrderByDescending(r => r.IsPinned)
            .ThenByDescending(r => r.CreatedAt)
            .Select(r => new SharedResourceDto(r.Id, r.Title, r.Description, r.Url, r.Category.ToString(), r.IsPinned, r.UploadedByUserId))
            .ToListAsync(cancellationToken);
    }
}

