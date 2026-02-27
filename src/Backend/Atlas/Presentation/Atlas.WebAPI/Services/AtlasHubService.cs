using Atlas.Application.Common.Interfaces;
using Atlas.WebAPI.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Atlas.WebAPI.Services;

public class AtlasHubService(IHubContext<AtlasHub> hubContext) : IAtlasHubService
{
    public async Task SendAlertAsync(Guid teamId, string alertType, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"team-{teamId}")
            .SendAsync("ReceiveAlert", new { AlertType = alertType, Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }

    public async Task SendToUserAsync(Guid userId, string method, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"user-{userId}")
            .SendAsync(method, new { Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }

    public async Task SendPresenceUpdateAsync(Guid teamId, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"team-{teamId}")
            .SendAsync("PresenceUpdated", new { Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }

    public async Task SendFocusStateAsync(Guid teamId, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"team-{teamId}")
            .SendAsync("FocusStateChanged", new { Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }

    public async Task SendJobCompletedAsync(Guid userId, string jobType, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"user-{userId}")
            .SendAsync("JobCompleted", new { JobType = jobType, Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }

    public async Task SendFeedUpdateAsync(Guid teamId, string eventType, object payload, CancellationToken ct)
    {
        await hubContext.Clients.Group($"team-{teamId}")
            .SendAsync("FeedUpdated", new { EventType = eventType, Payload = payload, Timestamp = DateTime.UtcNow }, ct);
    }
}

