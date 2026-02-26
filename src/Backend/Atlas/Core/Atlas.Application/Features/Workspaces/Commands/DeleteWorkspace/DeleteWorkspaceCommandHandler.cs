using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;

public class DeleteWorkspaceCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<DeleteWorkspaceCommandHandler> logger) : IRequestHandler<DeleteWorkspaceCommand>
{
    public async Task Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        logger.LogInformation("Deleting workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);
        
        var workspace = await context.Workspaces
            .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId && w.UserProfileId == userId, cancellationToken);

        if (workspace == null) throw new NotFoundException("Workspace", request.WorkspaceId);

        if (workspace.IsDefault)
        {
            var replacement = await context.Workspaces
                .Where(w => w.UserProfileId == userId && !w.IsDeleted && w.Id != workspace.Id)
                .OrderByDescending(w => w.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (replacement == null)
            {
                logger.LogWarning("Attempted to delete the only default workspace {WorkspaceId} for user {UserId}", request.WorkspaceId, userId);
                throw new BusinessRuleViolationException("Delete", "Cannot delete the only workspace. Please create another workspace before deleting this one.");
            }

            var otherDefaults = await context.Workspaces
                .Where(w => w.UserProfileId == userId && w.IsDefault && w.Id != workspace.Id && w.Id != replacement.Id)
                .ToListAsync(cancellationToken);

            foreach (var od in otherDefaults)
            {
                od.SetDefault(false);
                logger.LogDebug("Cleared stray default flag from workspace {WorkspaceId}", od.Id);
            }

            replacement.SetDefault(true);
            logger.LogInformation("Auto-selected workspace {ReplacementId} as new default for user {UserId} because default workspace {OldId} is being deleted", replacement.Id, userId, workspace.Id);
        }

        foreach (var link in workspace.WorkspaceIntegrations.ToList())
        {
            link.Delete();
            logger.LogDebug("Soft-deleted workspace-integration link {LinkId} for workspace {WorkspaceId}", link.Id, workspace.Id);
        }

        workspace.Delete(); 
        await context.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully deleted workspace {WorkspaceId}", request.WorkspaceId);
    }
}
