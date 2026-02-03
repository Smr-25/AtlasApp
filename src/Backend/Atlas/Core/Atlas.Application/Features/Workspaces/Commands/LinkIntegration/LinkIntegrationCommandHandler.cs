using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Workspaces.Commands.LinkIntegration;

public class LinkIntegrationCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IActivityService activityService
) : IRequestHandler<LinkIntegrationCommand, bool>
{
    public async Task<bool> Handle(LinkIntegrationCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var workspace = await applicationDbContext.Workspaces
            .Include(w => w.Persona)
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.Persona.UserId.Equals(userId),
                cancellationToken);

        if (workspace == null)
            throw new NotFoundException("Workspace not found");

        var integration = await applicationDbContext.Integrations
            .FirstOrDefaultAsync(i => i.Id == request.IntegrationId, cancellationToken);

        if (integration == null)
            throw new NotFoundException("Integration not found");

        if (workspace.PersonaId != integration.PersonaId)
            throw new ArgumentException("Workspace and Integration must belong to the same Persona.");

        var exists = await applicationDbContext.WorkspaceIntegrations
            .AnyAsync(wi => wi.WorkspaceId == request.WorkspaceId && wi.IntegrationId == request.IntegrationId,
                cancellationToken);

        if (exists) return true;

        var link = WorkspaceIntegration.Create(request.WorkspaceId, request.IntegrationId, request.Config);
        await applicationDbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        await activityService.LogAsync(
            Guid.Parse(userId!),
            "LinkIntegration",
            "Linked integration to workspace",
            request.WorkspaceId,
            cancellationToken
        );
        return true;
    }
}