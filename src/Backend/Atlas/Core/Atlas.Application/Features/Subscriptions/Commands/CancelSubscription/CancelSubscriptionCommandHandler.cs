using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IStripeService stripeService)
    : IRequestHandler<CancelSubscriptionCommand, bool>
{
    public async Task<bool> Handle(CancelSubscriptionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Subscription", userId);

        if (!string.IsNullOrEmpty(subscription.StripeSubscriptionId))
            await stripeService.CancelSubscriptionAsync(subscription.StripeSubscriptionId, cancellationToken);

        subscription.Cancel();
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}

