using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceTools;

public class GetWorkspaceToolsQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkspaceToolsQuery, List<WorkspaceToolDto>>
{
    public async Task<List<WorkspaceToolDto>> Handle(GetWorkspaceToolsQuery request,
        CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var hasAccess = await applicationDbContext.Workspaces
            .AnyAsync(w => w.Id == request.WorkspaceId && w.Persona.UserId.Equals(userId), cancellationToken);

        if (!hasAccess) return [];

        return await applicationDbContext.WorkspaceIntegrations
            .Where(wi => wi.WorkspaceId == request.WorkspaceId && !wi.IsDeleted)
            .Include(wi => wi.Integration)
            .Select(wi => new WorkspaceToolDto(
                wi.Id,
                wi.IntegrationId,
                wi.Integration.Name,
                wi.Integration.Provider.ToString(),
                wi.Config
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}