using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;

public class ToggleWorkspaceIntegrationHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<ToggleWorkspaceIntegrationCommand>
{
    public async Task Handle(ToggleWorkspaceIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var workspace = await applicationDbContext.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);
        
        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId && i.UserProfileId == userId, cancellationToken);
            
        if (integration == null) throw new NotFoundException("Integration", request.IntegrationId);

        var link = await applicationDbContext.WorkspaceIntegrations
            .FirstOrDefaultAsync(wi => wi.WorkspaceId == request.WorkspaceId && wi.IntegrationId == request.IntegrationId, cancellationToken);

        if (request.Enable)
        {
            if (link == null)
            {
                link = new WorkspaceIntegration
                {
                    WorkspaceId = request.WorkspaceId,
                    IntegrationId = request.IntegrationId
                };
                await applicationDbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
            }
        }
        else
        {
            if (link != null)
            {
                applicationDbContext.WorkspaceIntegrations.Remove(link);
            }
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}