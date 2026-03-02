using Atlas.Domain.Enums;

namespace Atlas.Application.Common.Interfaces;

public interface IWorkspaceAccessService
{
    Task<WorkspaceMemberRole> ValidateAccessAsync(Guid workspaceId, Guid userId, WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer, CancellationToken cancellationToken = default);
    
    Task<Guid> GetAndValidateWorkspaceAsync(Guid userId, WorkspaceMemberRole minimumRole = WorkspaceMemberRole.Viewer, CancellationToken cancellationToken = default);
    
    Task<bool> CanUseIntegrationAsync(Guid workspaceId, Guid integrationId, Guid userId, CancellationToken cancellationToken = default);
}
