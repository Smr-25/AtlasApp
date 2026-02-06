using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaces;

public class GetWorkspacesQueryHandler(
    IApplicationDbContext context, 
    IMapper mapper,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspacesQuery, List<WorkspaceDto>>
{
    public async Task<List<WorkspaceDto>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        return await context.Workspaces.Include(w => w.WorkspaceIntegrations)
            .ThenInclude(wi => wi.Integration)
            .Where(w => w.UserProfileId == userId && !w.IsDeleted)
            .OrderByDescending(w => w.IsDefault) 
            .ThenBy(w => w.Name)
            .ProjectTo<WorkspaceDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}

