using Atlas.Application.Features.Subscriptions.Commands.CancelSubscription;
using Atlas.Application.Features.Subscriptions.Commands.CreateCheckoutSession;
using Atlas.Application.Features.Subscriptions.Dtos;
using Atlas.Application.Features.Subscriptions.Queries.GetCurrentSubscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SubscriptionsController : ApiControllerBase
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var result = await Mediator.Send(new GetCurrentSubscriptionQuery());
        return OkResponse(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutSessionCommand command)
    {
        var checkoutUrl = await Mediator.Send(command);
        return OkResponse(new { Url = checkoutUrl });
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> Cancel()
    {
        var result = await Mediator.Send(new CancelSubscriptionCommand());
        return OkResponse(result);
    }
}

