using Atlas.Application.Features.SecOpsInsights.Queries.GetAverageResponseTime;
using Atlas.Application.Features.SecOpsInsights.Queries.GetOpenPortsGraph;
using Atlas.Application.Features.SecOpsInsights.Queries.GetScannedBytes;
using Atlas.Application.Features.SecOpsInsights.Queries.GetSecurityScore;
using Atlas.Application.Features.SecOpsInsights.Queries.GetThreatsBlocked;
using Atlas.Application.Features.SecOpsInsights.Queries.GetVulnerabilitiesPatched;
using Atlas.Application.Features.SecOpsInsights.Queries.GetZeroIncidentStreak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SecOpsInsightsController : ApiControllerBase
{
    [HttpGet("threats-blocked")]
    public async Task<IActionResult> GetThreatsBlocked([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetThreatsBlockedQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("vulnerabilities-patched")]
    public async Task<IActionResult> GetVulnerabilitiesPatched([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetVulnerabilitiesPatchedQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("avg-response-time")]
    public async Task<IActionResult> GetAverageResponseTime([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetAverageResponseTimeQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("security-score")]
    public async Task<IActionResult> GetSecurityScore()
    {
        var result = await Mediator.Send(new GetSecurityScoreQuery());
        return OkResponse(result);
    }

    [HttpGet("zero-incident-streak")]
    public async Task<IActionResult> GetZeroIncidentStreak()
    {
        var result = await Mediator.Send(new GetZeroIncidentStreakQuery());
        return OkResponse(result);
    }

    [HttpGet("scanned-bytes")]
    public async Task<IActionResult> GetScannedBytes([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetScannedBytesQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("open-ports-graph")]
    public async Task<IActionResult> GetOpenPortsGraph([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetOpenPortsGraphQuery(from, to));
        return OkResponse(result);
    }
}

