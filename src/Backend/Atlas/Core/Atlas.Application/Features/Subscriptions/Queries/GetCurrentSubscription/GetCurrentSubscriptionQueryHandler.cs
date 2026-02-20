using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Subscriptions.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Subscriptions.Queries.GetCurrentSubscription;

public class GetCurrentSubscriptionQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetCurrentSubscriptionQuery, SubscriptionDto>
{
    public async Task<SubscriptionDto> Handle(GetCurrentSubscriptionQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Subscription", userId);

        return new SubscriptionDto(
            subscription.Id,
            subscription.Tier.ToString(),
            subscription.Status.ToString(),
            subscription.MaxWorkspaces,
            subscription.MaxIntegrations,
            subscription.HasCustomHotkeys,
            subscription.CurrentPeriodEnd
        );
    }
}

