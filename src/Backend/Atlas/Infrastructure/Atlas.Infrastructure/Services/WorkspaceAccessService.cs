using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class WorkspaceAccessService(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService) : IWorkspaceAccessService
{
    public async Task<WorkspaceMemberRole> ValidateAccessAsync(
        Guid workspaceId, Guid userId, 
        WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer, 
        CancellationToken cancellationToken = default)
    {
        // Workspace owner has implicit full access
        var workspace = await dbContext.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted, cancellationToken);

        if (workspace == null)
            throw new NotFoundException("Workspace", workspaceId);

        // Owner always has full access
        if (workspace.UserProfileId == userId)
            return WorkspaceMemberRole.Owner;

        // Check membership
        var member = await dbContext.WorkspaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId 
                                       && wm.UserId == userId 
                                       && !wm.IsDeleted, cancellationToken);

        if (member == null)
            throw new ForbiddenException("You do not have access to this workspace.");

        // Lower enum value = higher privilege (Owner=1, Admin=2, Editor=3, Viewer=4)
        if (member.Role > minimumRole)
            throw new ForbiddenException($"This action requires at least {minimumRole} role in the workspace.");

        return member.Role;
    }

    public async Task<Guid> GetAndValidateWorkspaceAsync(
        Guid userId,
        WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer,
        CancellationToken cancellationToken = default)
    {
        var workspaceId = currentUserService.GetRequiredWorkspaceId();
        await ValidateAccessAsync(workspaceId, userId, minimumRole, cancellationToken);
        return workspaceId;
    }

    public async Task<bool> CanUseIntegrationAsync(
        Guid workspaceId, Guid integrationId, Guid userId,
        CancellationToken cancellationToken = default)
    {
        var integration = await dbContext.Integrations
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == integrationId && !i.IsDeleted, cancellationToken);

        if (integration == null)
            return false;

        // Workspace-scope integrations can be used by anyone with workspace access
        if (integration.Scope == IntegrationScope.Workspace)
            return true;

        // Personal-scope integrations can only be used by their owner
        // Other workspace members must connect their own integration
        return integration.UserProfileId == userId;
    }
}

