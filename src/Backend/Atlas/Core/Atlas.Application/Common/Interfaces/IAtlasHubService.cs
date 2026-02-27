namespace Atlas.Application.Common.Interfaces;

public interface IAtlasHubService
{
    Task SendAlertAsync(Guid teamId, string alertType, object payload, CancellationToken ct = default);
    Task SendToUserAsync(Guid userId, string method, object payload, CancellationToken ct = default);
    Task SendPresenceUpdateAsync(Guid teamId, object payload, CancellationToken ct = default);
    Task SendFocusStateAsync(Guid teamId, object payload, CancellationToken ct = default);
    Task SendJobCompletedAsync(Guid userId, string jobType, object payload, CancellationToken ct = default);
    Task SendFeedUpdateAsync(Guid teamId, string eventType, object payload, CancellationToken ct = default);
}

