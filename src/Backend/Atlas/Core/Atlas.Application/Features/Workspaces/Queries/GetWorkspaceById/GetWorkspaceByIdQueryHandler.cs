using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Workspaces.Dtos;
using Atlas.Domain.Enums;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;

public class GetWorkspaceByIdQueryHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<GetWorkspaceByIdQueryHandler> logger) : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceDto>
{
    public async Task<WorkspaceDto> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogDebug("Fetching workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);

        var role = await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, cancellationToken: cancellationToken);

        var workspace = await context.Workspaces
            .Include(w => w.Members.Where(m => !m.IsDeleted))
            .Include(w => w.WorkspaceIntegrations.Where(wi => wi.Enabled))
                .ThenInclude(wi => wi.Integration)
            .Where(w => w.Id == request.WorkspaceId && !w.IsDeleted)
            .FirstOrDefaultAsync(cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        var dto = mapper.Map<WorkspaceDto>(workspace);
        dto.MyRole = role;
        
        logger.LogDebug("Successfully retrieved workspace {WorkspaceId}", request.WorkspaceId);
        return dto;
    }
}

