using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;

namespace Atlas.Infrastructure.Services;

public class ActivityService(IApplicationDbContext applicationDbContext) : IActivityService
{
    public async Task LogAsync(Guid userId, string actionType, string description, Guid? workspaceId = null,
        CancellationToken cancellationToken = default)
    {
        var activity = UserActivity.Create(userId, actionType, description, workspaceId);
        await applicationDbContext.UserActivities.AddAsync(activity, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}