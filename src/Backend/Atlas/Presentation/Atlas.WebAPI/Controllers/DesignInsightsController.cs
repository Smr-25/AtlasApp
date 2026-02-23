using Atlas.Application.Features.DesignInsights.Queries.GetAssetsOptimized;
using Atlas.Application.Features.DesignInsights.Queries.GetColorTrend;
using Atlas.Application.Features.DesignInsights.Queries.GetDesignDebt;
using Atlas.Application.Features.DesignInsights.Queries.GetHandoffsCompleted;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DesignInsightsController : ApiControllerBase
{
    [HttpGet("assets-optimized")]
    public async Task<IActionResult> GetAssetsOptimized()
    {
        var result = await Mediator.Send(new GetAssetsOptimizedQuery());
        return OkResponse(result);
    }

    [HttpGet("handoffs")]
    public async Task<IActionResult> GetHandoffsCompleted([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetHandoffsCompletedQuery(from, to));
        return OkResponse(new { Count = result });
    }

    [HttpGet("color-trends")]
    public async Task<IActionResult> GetColorTrend()
    {
        var result = await Mediator.Send(new GetColorTrendQuery());
        return OkResponse(result);
    }

    [HttpGet("design-debt")]
    public async Task<IActionResult> GetDesignDebt()
    {
        var result = await Mediator.Send(new GetDesignDebtQuery());
        return OkResponse(new { Count = result });
    }
}

