using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Services;

public class SubscriptionGuardService(IApplicationDbContext dbContext) : ISubscriptionGuardService
{
    public async Task<bool> CanCreateWorkspaceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

        if (subscription == null) return false;
        if (!subscription.IsActive) return false;
        if (subscription.Tier is SubscriptionTier.Pro or SubscriptionTier.Team) return true;

        var currentWorkspaceCount = await dbContext.Workspaces
            .CountAsync(w => w.UserProfileId == userId && !w.IsDeleted, cancellationToken);

        return currentWorkspaceCount < subscription.MaxWorkspaces;
    }

    public async Task<bool> CanAddIntegrationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

        if (subscription == null) return false;
        if (!subscription.IsActive) return false;
        if (subscription.Tier is SubscriptionTier.Pro or SubscriptionTier.Team) return true;

        var currentIntegrationCount = await dbContext.Integrations
            .CountAsync(i => i.UserProfileId == userId && !i.IsDeleted, cancellationToken);

        return currentIntegrationCount < subscription.MaxIntegrations;
    }

    public async Task<bool> HasCustomHotkeysAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

        return subscription is { IsActive: true, HasCustomHotkeys: true };
    }

    public async Task<bool> HasTeamFeaturesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

        return subscription is { IsActive: true, Tier: SubscriptionTier.Team };
    }

    public async Task ThrowIfCannotCreateWorkspaceAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!await CanCreateWorkspaceAsync(userId, cancellationToken))
            throw new ForbiddenException("Workspace limit reached. Upgrade your subscription to create more workspaces.");
    }

    public async Task ThrowIfCannotAddIntegrationAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!await CanAddIntegrationAsync(userId, cancellationToken))
            throw new ForbiddenException("Integration limit reached. Upgrade your subscription to add more integrations.");
    }
}

