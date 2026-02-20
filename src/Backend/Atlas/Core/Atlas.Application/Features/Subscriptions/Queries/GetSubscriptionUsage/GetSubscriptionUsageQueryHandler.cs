using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Subscriptions.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Subscriptions.Queries.GetSubscriptionUsage;

public class GetSubscriptionUsageQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ISubscriptionGuardService subscriptionGuard)
    : IRequestHandler<GetSubscriptionUsageQuery, SubscriptionUsageDto>
{
    public async Task<SubscriptionUsageDto> Handle(GetSubscriptionUsageQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Subscription", userId);

        var currentWorkspaces = await dbContext.Workspaces
            .CountAsync(w => w.UserProfileId == userId && !w.IsDeleted, cancellationToken);

        var currentIntegrations = await dbContext.Integrations
            .CountAsync(i => i.UserProfileId == userId && !i.IsDeleted, cancellationToken);

        var canCreateWorkspace = await subscriptionGuard.CanCreateWorkspaceAsync(userId, cancellationToken);
        var canAddIntegration = await subscriptionGuard.CanAddIntegrationAsync(userId, cancellationToken);

        return new SubscriptionUsageDto(
            subscription.Tier.ToString(),
            subscription.Status.ToString(),
            subscription.MaxWorkspaces,
            currentWorkspaces,
            subscription.MaxIntegrations,
            currentIntegrations,
            subscription.HasCustomHotkeys,
            canCreateWorkspace,
            canAddIntegration,
            subscription.CurrentPeriodEnd
        );
    }
}

