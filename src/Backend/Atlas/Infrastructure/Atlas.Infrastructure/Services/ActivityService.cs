using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Atlas.Infrastructure.Services;

public class ActivityService(
    IApplicationDbContext applicationDbContext,
    ILogger<ActivityService> logger) : IActivityService
{
    public async Task LogAsync(Guid userId, string actionType, string description, Guid? workspaceId = null,
        CancellationToken cancellationToken = default)
    {
        var activity = UserActivity.Create(userId, actionType, description, workspaceId);
        await applicationDbContext.UserActivities.AddAsync(activity, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogDebug("Logged activity: {ActionType} for user {UserId}", actionType, userId);
    }
}