using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Subscriptions.Commands.CreatePortalSession;

public class CreatePortalSessionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IStripeService stripeService)
    : IRequestHandler<CreatePortalSessionCommand, string>
{
    public async Task<string> Handle(CreatePortalSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Subscription", userId);

        if (string.IsNullOrEmpty(subscription.StripeCustomerId))
            throw new BadRequestException("No Stripe customer found. Please subscribe first.");

        var portalUrl = await stripeService.CreatePortalSessionAsync(
            subscription.StripeCustomerId, request.ReturnUrl, cancellationToken);

        return portalUrl;
    }
}

