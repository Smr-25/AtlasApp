using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IWorkspaceAccessService
{
    /// <summary>
    /// Verifies that the current user has access to the specified workspace with at least the given role.
    /// Throws ForbiddenException if access is denied.
    /// </summary>
    Task<WorkspaceMemberRole> ValidateAccessAsync(Guid workspaceId, Guid userId, WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the workspace ID from X-Workspace-Id header, validates access, and returns it.
    /// Throws if header is missing or user has no access.
    /// </summary>
    Task<Guid> GetAndValidateWorkspaceAsync(Guid userId, WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a shared workspace integration can be used by the requesting user.
    /// For Personal scope integrations in shared workspaces, the user must connect their own.
    /// </summary>
    Task<bool> CanUseIntegrationAsync(Guid workspaceId, Guid integrationId, Guid userId, CancellationToken cancellationToken = default);
}

