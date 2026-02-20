using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Atlas.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    IStripeService stripeService,
    UserManager<AppUser> userManager,
    IOptions<StripeSettings> stripeSettings)
    : IRequestHandler<CreateCheckoutSessionCommand, string>
{
    private readonly StripeSettings _stripeSettings = stripeSettings.Value;

    public async Task<string> Handle(CreateCheckoutSessionCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Subscription", userId);

        // Create Stripe customer if not exists
        var customerId = subscription.StripeCustomerId;
        if (string.IsNullOrEmpty(customerId))
        {
            customerId = await stripeService.CreateCustomerAsync(user.Email!, user.FullName, cancellationToken);
            subscription.UpdateStripeInfo(customerId, subscription.StripeSubscriptionId ?? "");
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var priceId = request.Tier switch
        {
            SubscriptionTier.Pro => _stripeSettings.ProPriceId,
            SubscriptionTier.Team => _stripeSettings.TeamPriceId,
            _ => throw new BadRequestException("Cannot create checkout for Free tier.")
        };

        var checkoutUrl = await stripeService.CreateCheckoutSessionAsync(
            customerId, priceId, request.SuccessUrl, request.CancelUrl, cancellationToken);

        return checkoutUrl;
    }
}

