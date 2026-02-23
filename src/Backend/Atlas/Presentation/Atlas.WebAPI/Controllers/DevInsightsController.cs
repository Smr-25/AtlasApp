using Atlas.Application.Features.DevInsights.Queries.GetDeploymentSuccessRate;
using Atlas.Application.Features.DevInsights.Queries.GetFocusHeatmap;
using Atlas.Application.Features.DevInsights.Queries.GetPeakHours;
using Atlas.Application.Features.DevInsights.Queries.GetTechDebt;
using Atlas.Application.Features.DevInsights.Queries.GetTimeSaved;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DevInsightsController : ApiControllerBase
{
    [HttpGet("time-saved")]
    public async Task<IActionResult> GetTimeSaved([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetTimeSavedQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("focus-heatmap")]
    public async Task<IActionResult> GetFocusHeatmap([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetFocusHeatmapQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("tech-debt")]
    public async Task<IActionResult> GetTechDebt([FromQuery] string projectPath)
    {
        var result = await Mediator.Send(new GetTechDebtQuery(projectPath));
        return OkResponse(result);
    }

    [HttpGet("deployment-success-rate")]
    public async Task<IActionResult> GetDeploymentSuccessRate([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetDeploymentSuccessRateQuery(from, to));
        return OkResponse(result);
    }

    [HttpGet("peak-hours")]
    public async Task<IActionResult> GetPeakHours([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetPeakHoursQuery(from, to));
        return OkResponse(result);
    }
}

