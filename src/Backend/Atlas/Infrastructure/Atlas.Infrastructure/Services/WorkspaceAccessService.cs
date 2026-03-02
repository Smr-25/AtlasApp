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
        var workspace = await dbContext.Workspaces
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == workspaceId && !w.IsDeleted, cancellationToken);

        if (workspace == null)
            throw new NotFoundException("Workspace", workspaceId);

        if (workspace.UserProfileId == userId)
            return WorkspaceMemberRole.Owner;

        var member = await dbContext.WorkspaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(wm => wm.WorkspaceId == workspaceId 
                                       && wm.UserId == userId 
                                       && !wm.IsDeleted, cancellationToken);

        if (member == null)
            throw new ForbiddenException("You do not have access to this workspace.");

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

        if (integration.Scope == IntegrationScope.Workspace)
            return true;

        return integration.UserProfileId == userId;
    }
}
