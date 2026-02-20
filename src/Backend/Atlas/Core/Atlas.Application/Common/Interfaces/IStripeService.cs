namespace Atlas.Application.Common.Interfaces;

public interface IStripeService
{
    Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken = default);
    Task<string> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl, CancellationToken cancellationToken = default);
    Task<string> CreatePortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default);
    Task CancelSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);
}

