using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaces;

public class GetWorkspacesQueryHandler(
    IApplicationDbContext context, 
    IMapper mapper,
    ICurrentUserService currentUserService,
    ILogger<GetWorkspacesQueryHandler> logger) : IRequestHandler<GetWorkspacesQuery, List<WorkspaceDto>>
{
    public async Task<List<WorkspaceDto>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching workspaces for user {UserId}", userId);
        
        var workspaces = await context.Workspaces
            .Where(w => w.UserProfileId == userId && !w.IsDeleted)
            .OrderByDescending(w => w.IsDefault) 
            .ThenBy(w => w.Name)
            .ProjectTo<WorkspaceDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        
        logger.LogDebug("Retrieved {Count} workspaces for user {UserId}", workspaces.Count, userId);
        return workspaces;
    }
}

