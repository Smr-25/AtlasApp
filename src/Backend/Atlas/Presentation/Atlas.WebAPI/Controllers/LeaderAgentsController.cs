using Atlas.Application.Features.LeaderAgents.Commands.NagPrReviews;
using Atlas.Application.Features.LeaderAgents.Commands.PingGhostMembers;
using Atlas.Application.Features.LeaderAgents.Queries.CatchUnassignedBugs;
using Atlas.Application.Features.LeaderAgents.Queries.CheckMilestoneCelebration;
using Atlas.Application.Features.LeaderAgents.Queries.DetectBurnoutRisk;
using Atlas.Application.Features.LeaderAgents.Queries.DetectScopeCreep;
using Atlas.Application.Features.LeaderAgents.Queries.PredictBottleneck;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LeaderAgentsController : ApiControllerBase
{
    [HttpGet("bottleneck/{teamId:guid}")]
    public async Task<IActionResult> PredictBottleneck(Guid teamId)
    {
        var result = await Mediator.Send(new PredictBottleneckQuery(teamId));
        return OkResponse(result);
    }

    [HttpGet("burnout-risk/{teamId:guid}")]
    public async Task<IActionResult> DetectBurnoutRisk(Guid teamId)
    {
        var result = await Mediator.Send(new DetectBurnoutRiskQuery(teamId));
        return OkResponse(result);
    }

    [HttpGet("scope-creep/{teamId:guid}")]
    public async Task<IActionResult> DetectScopeCreep(Guid teamId, [FromQuery] string sprintId)
    {
        var result = await Mediator.Send(new DetectScopeCreepQuery(teamId, sprintId));
        return OkResponse(result);
    }

    [HttpPost("pr-review-nag")]
    public async Task<IActionResult> NagPrReviews([FromBody] NagPrReviewsCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpGet("unassigned-bugs/{teamId:guid}")]
    public async Task<IActionResult> CatchUnassignedBugs(Guid teamId)
    {
        var result = await Mediator.Send(new CatchUnassignedBugsQuery(teamId));
        return OkResponse(result);
    }

    [HttpPost("ghost-members")]
    public async Task<IActionResult> PingGhostMembers([FromBody] PingGhostMembersCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpGet("milestone/{teamId:guid}")]
    public async Task<IActionResult> CheckMilestoneCelebration(Guid teamId)
    {
        var result = await Mediator.Send(new CheckMilestoneCelebrationQuery(teamId));
        return OkResponse(result);
    }
}

