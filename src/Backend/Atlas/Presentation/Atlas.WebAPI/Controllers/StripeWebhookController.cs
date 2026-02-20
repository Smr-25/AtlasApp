using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/stripe/webhook")]
public class StripeWebhookController(
    IApplicationDbContext dbContext,
    IOptions<StripeSettings> stripeSettings) : ControllerBase
{
    private readonly StripeSettings _stripeSettings = stripeSettings.Value;

    [HttpPost]
    public async Task<IActionResult> Handle()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"],
                _stripeSettings.WebhookSecret);
        }
        catch (StripeException)
        {
            return BadRequest("Invalid Stripe signature.");
        }

        switch (stripeEvent.Type)
        {
            case EventTypes.CheckoutSessionCompleted:
                await HandleCheckoutCompleted(stripeEvent);
                break;
            case EventTypes.InvoicePaid:
                await HandleInvoicePaid(stripeEvent);
                break;
            case EventTypes.CustomerSubscriptionUpdated:
                await HandleSubscriptionUpdated(stripeEvent);
                break;
            case EventTypes.CustomerSubscriptionDeleted:
                await HandleSubscriptionDeleted(stripeEvent);
                break;
        }

        return Ok();
    }

    private async Task HandleCheckoutCompleted(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Session session) return;

        var customerId = session.CustomerId;
        var subscriptionId = session.SubscriptionId;

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeCustomerId == customerId);

        if (subscription == null) return;

        // Determine tier from Stripe subscription
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        var stripeSubService = new SubscriptionService();
        var stripeSub = await stripeSubService.GetAsync(subscriptionId);
        
        var priceId = stripeSub.Items.Data[0].Price.Id;
        var tier = priceId == _stripeSettings.ProPriceId ? SubscriptionTier.Pro : SubscriptionTier.Team;

        subscription.UpgradeTo(tier, customerId, subscriptionId, stripeSub.CurrentPeriodEnd);
        await dbContext.SaveChangesAsync();
    }

    private async Task HandleInvoicePaid(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Invoice invoice) return;

        var customerId = invoice.CustomerId;

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeCustomerId == customerId);

        if (subscription == null) return;

        subscription.Renew(invoice.Lines.Data[0].Period.End);
        await dbContext.SaveChangesAsync();
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Subscription stripeSub) return;

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSub.Id);

        if (subscription == null) return;

        if (stripeSub.Status == "past_due")
        {
            subscription.MarkPastDue();
        }
        else if (stripeSub.Status == "active")
        {
            subscription.Renew(stripeSub.CurrentPeriodEnd);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent)
    {
        if (stripeEvent.Data.Object is not Subscription stripeSub) return;

        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.StripeSubscriptionId == stripeSub.Id);

        if (subscription == null) return;

        subscription.Cancel();
        await dbContext.SaveChangesAsync();
    }
}

