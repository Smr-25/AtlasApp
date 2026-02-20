namespace Atlas.Application.Common.Interfaces;

public interface ISubscriptionGuardService
{
    Task<bool> CanCreateWorkspaceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanAddIntegrationAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasCustomHotkeysAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasTeamFeaturesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ThrowIfCannotCreateWorkspaceAsync(Guid userId, CancellationToken cancellationToken = default);
    Task ThrowIfCannotAddIntegrationAsync(Guid userId, CancellationToken cancellationToken = default);
}

