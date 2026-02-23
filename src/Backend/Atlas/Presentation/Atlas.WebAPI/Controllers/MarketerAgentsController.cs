using Atlas.Application.Features.MarketerAgents.Commands.AppendAutoUtm;
using Atlas.Application.Features.MarketerAgents.Commands.ResendLowOpenRate;
using Atlas.Application.Features.MarketerAgents.Commands.WarnBudgetBleed;
using Atlas.Application.Features.MarketerAgents.Queries.DetectBrokenLinks;
using Atlas.Application.Features.MarketerAgents.Queries.DetectCartAbandonment;
using Atlas.Application.Features.MarketerAgents.Queries.DetectCompetitorPriceDrop;
using Atlas.Application.Features.MarketerAgents.Queries.GetViralTrends;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class MarketerAgentsController : ApiControllerBase
{
    [HttpPost("budget-bleed")]
    public async Task<IActionResult> WarnBudgetBleed()
    {
        var result = await Mediator.Send(new WarnBudgetBleedCommand());
        return OkResponse(result);
    }

    [HttpPost("broken-links")]
    public async Task<IActionResult> DetectBrokenLinks([FromBody] DetectBrokenLinksQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("viral-trends")]
    public async Task<IActionResult> GetViralTrends([FromBody] GetViralTrendsQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("competitor-price-drop")]
    public async Task<IActionResult> DetectCompetitorPriceDrop([FromBody] DetectCompetitorPriceDropQuery query)
    {
        var result = await Mediator.Send(query);
        return OkResponse(result);
    }

    [HttpPost("resend-low-open")]
    public async Task<IActionResult> ResendLowOpenRate([FromBody] ResendLowOpenRateCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Result = result });
    }

    [HttpPost("auto-utm")]
    public async Task<IActionResult> AppendAutoUtm([FromBody] AppendAutoUtmCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { UtmUrl = result });
    }

    [HttpGet("cart-abandonment")]
    public async Task<IActionResult> DetectCartAbandonment()
    {
        var result = await Mediator.Send(new DetectCartAbandonmentQuery());
        return OkResponse(result);
    }
}

