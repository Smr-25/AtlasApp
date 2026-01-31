using Atlas.Application.Features.Timelines.Queries.GetMyTimeline;
using Atlas.Application.Features.Timelines.Queries.GetTimelineByDateRange;
using Atlas.Application.Features.Timelines.Queries.GetTimelineByEventType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TimelineController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMyTimeline()
    {
        var result = await mediator.Send(new GetMyTimelineQuery());
        return Ok(result);
    }

    [HttpGet("range")]
    public async Task<IActionResult> GetTimelineByDateRange([FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var result = await mediator.Send(new GetTimelineByDateRangeQuery(startDate, endDate));
        return Ok(result);
    }

    [HttpGet("type/{eventType}")]
    public async Task<IActionResult> GetTimelineByEventType([FromRoute] string eventType)
    {
        var result = await mediator.Send(new GetTimelineByEventTypeQuery(eventType));
        return Ok(result);
    }
}