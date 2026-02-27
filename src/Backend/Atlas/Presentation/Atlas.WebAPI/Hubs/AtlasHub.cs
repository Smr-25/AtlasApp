using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Atlas.WebAPI.Hubs;

[Authorize]
public class AtlasHub : Hub
{
    public async Task JoinTeam(string teamId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"team-{teamId}");
    }

    public async Task LeaveTeam(string teamId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"team-{teamId}");
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");

        await base.OnDisconnectedAsync(exception);
    }
}

