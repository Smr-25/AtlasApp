using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Atlas.Infrastructure.Services;

public class StripeService(IOptions<StripeSettings> stripeSettings) : IStripeService
{
    private readonly StripeSettings _settings = stripeSettings.Value;

    public async Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _settings.SecretKey;
        
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };

        var service = new CustomerService();
        var customer = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return customer.Id;
    }

    public async Task<string> CreateCheckoutSessionAsync(string customerId, string priceId, 
        string successUrl, string cancelUrl, CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _settings.SecretKey;

        var options = new SessionCreateOptions
        {
            Customer = customerId,
            PaymentMethodTypes = ["card"],
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Price = priceId,
                    Quantity = 1
                }
            ],
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return session.Url;
    }

    public async Task<string> CreatePortalSessionAsync(string customerId, string returnUrl, 
        CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _settings.SecretKey;

        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl
        };

        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return session.Url;
    }

    public async Task CancelSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        StripeConfiguration.ApiKey = _settings.SecretKey;

        var service = new SubscriptionService();
        await service.CancelAsync(subscriptionId, cancellationToken: cancellationToken);
    }
}

