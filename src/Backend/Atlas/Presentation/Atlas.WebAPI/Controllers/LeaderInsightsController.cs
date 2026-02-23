using Atlas.Application.Features.LeaderInsights.Queries.GetBlockedTime;
using Atlas.Application.Features.LeaderInsights.Queries.GetCostPerFeature;
using Atlas.Application.Features.LeaderInsights.Queries.GetMeetingsAvoided;
using Atlas.Application.Features.LeaderInsights.Queries.GetReviewTurnaround;
using Atlas.Application.Features.LeaderInsights.Queries.GetSprintVelocity;
using Atlas.Application.Features.LeaderInsights.Queries.GetTeamMood;
using Atlas.Application.Features.LeaderInsights.Queries.GetTopContributor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LeaderInsightsController : ApiControllerBase
{
    [HttpGet("sprint-velocity")]
    public async Task<IActionResult> GetSprintVelocity([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetSprintVelocityQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("meetings-avoided")]
    public async Task<IActionResult> GetMeetingsAvoided([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetMeetingsAvoidedQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("blocked-time")]
    public async Task<IActionResult> GetBlockedTime([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetBlockedTimeQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("cost-per-feature")]
    public async Task<IActionResult> GetCostPerFeature([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetCostPerFeatureQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("review-turnaround")]
    public async Task<IActionResult> GetReviewTurnaround([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetReviewTurnaroundQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("top-contributor")]
    public async Task<IActionResult> GetTopContributor([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetTopContributorQuery(teamId, from, to));
        return OkResponse(result);
    }

    [HttpGet("team-mood")]
    public async Task<IActionResult> GetTeamMood([FromQuery] Guid teamId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var result = await Mediator.Send(new GetTeamMoodQuery(teamId, from, to));
        return OkResponse(result);
    }
}

