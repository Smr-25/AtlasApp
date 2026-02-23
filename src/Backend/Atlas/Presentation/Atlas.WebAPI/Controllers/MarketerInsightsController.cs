using Atlas.Application.Features.MarketerInsights.Queries.GetAbTestWinRate;
using Atlas.Application.Features.MarketerInsights.Queries.GetAudienceSentiment;
using Atlas.Application.Features.MarketerInsights.Queries.GetLeadsGenerated;
using Atlas.Application.Features.MarketerInsights.Queries.GetPeakEngagementHours;
using Atlas.Application.Features.MarketerInsights.Queries.GetTimeSavedOnReporting;
using Atlas.Application.Features.MarketerInsights.Queries.GetTotalRoas;
using Atlas.Application.Features.MarketerInsights.Queries.GetZombieAdsKilled;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class MarketerInsightsController : ApiControllerBase
{
    [HttpGet("total-roas")]
    public async Task<IActionResult> GetTotalRoas([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetTotalRoasQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("leads-generated")]
    public async Task<IActionResult> GetLeadsGenerated([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetLeadsGeneratedQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("zombie-ads-killed")]
    public async Task<IActionResult> GetZombieAdsKilled([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetZombieAdsKilledQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("ab-test-win-rate")]
    public async Task<IActionResult> GetAbTestWinRate([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetAbTestWinRateQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("peak-engagement")]
    public async Task<IActionResult> GetPeakEngagementHours([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetPeakEngagementHoursQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("audience-sentiment")]
    public async Task<IActionResult> GetAudienceSentiment([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetAudienceSentimentQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("time-saved-reporting")]
    public async Task<IActionResult> GetTimeSavedOnReporting([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetTimeSavedOnReportingQuery(from, to));
        return OkResponse(result);
    }
}

