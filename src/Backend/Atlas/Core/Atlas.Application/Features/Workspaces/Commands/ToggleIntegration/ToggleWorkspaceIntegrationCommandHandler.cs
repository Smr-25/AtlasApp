using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;

public class ToggleWorkspaceIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IWorkspaceAccessService workspaceAccess,
    ILogger<ToggleWorkspaceIntegrationCommandHandler> logger)
    : IRequestHandler<ToggleWorkspaceIntegrationCommand>
{
    public async Task Handle(ToggleWorkspaceIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Toggling integration {IntegrationId} for workspace {WorkspaceId}, Enable: {Enable}", 
            request.IntegrationId, request.WorkspaceId, request.Enable);

        await workspaceAccess.ValidateAccessAsync(request.WorkspaceId, userId, WorkspaceMemberRole.Editor, cancellationToken);

        var workspace = await applicationDbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && !w.IsDeleted, cancellationToken);
        
        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && !i.IsDeleted, cancellationToken);
            
        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        if (integration.Scope == IntegrationScope.Personal && integration.UserProfileId != userId)
            throw new ForbiddenException("Cannot toggle another user's personal integration. The user must connect their own.");

        var link = await applicationDbContext.WorkspaceIntegrations
            .FirstOrDefaultAsync(wi => wi.WorkspaceId == request.WorkspaceId && wi.IntegrationId == request.IntegrationId, cancellationToken);

        if (request.Enable)
        {
            if (link == null)
            {
                link = new WorkspaceIntegration
                {
                    WorkspaceId = request.WorkspaceId,
                    IntegrationId = request.IntegrationId,
                    Enabled = true
                };
                await applicationDbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
                logger.LogDebug("Created new workspace-integration link");
            }
            else if (!link.Enabled)
            {
                link.Enabled = true;
                logger.LogDebug("Re-enabled existing workspace-integration link");
            }
        }
        else
        {
            if (link != null && link.Enabled)
            {
                link.Enabled = false;
                logger.LogDebug("Disabled workspace-integration link");
            }
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully toggled integration for workspace");
    }
}