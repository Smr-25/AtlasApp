using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Enums;
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
        
        // User-in member olduğu bütün workspace ID-ları (öz yaratdıqları + shared olanlar)
        var memberWorkspaceIds = await context.WorkspaceMembers
            .Where(wm => wm.UserId == userId && !wm.IsDeleted)
            .Select(wm => wm.WorkspaceId)
            .ToListAsync(cancellationToken);
        
        var workspaces = await context.Workspaces
            .Include(w => w.Members.Where(m => !m.IsDeleted))
            .Include(w => w.WorkspaceIntegrations.Where(wi => wi.Enabled))
                .ThenInclude(wi => wi.Integration)
            .Where(w => (w.UserProfileId == userId || memberWorkspaceIds.Contains(w.Id)) && !w.IsDeleted)
            .OrderByDescending(w => w.IsDefault) 
            .ThenBy(w => w.Name)
            .ToListAsync(cancellationToken);
        
        var result = workspaces.Select(w =>
        {
            var dto = mapper.Map<WorkspaceDto>(w);
            var member = w.Members.FirstOrDefault(m => m.UserId == userId);
            dto.MyRole = member?.Role ?? (w.UserProfileId == userId ? WorkspaceMemberRole.Owner : WorkspaceMemberRole.Viewer);
            return dto;
        }).ToList();
        
        logger.LogDebug("Retrieved {Count} workspaces for user {UserId}", result.Count, userId);
        return result;
    }
}
