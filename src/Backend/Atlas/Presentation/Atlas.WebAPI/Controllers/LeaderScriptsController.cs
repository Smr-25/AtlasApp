using Atlas.Application.Features.LeaderScripts.Commands.RunBlockedTaskBlaster;
using Atlas.Application.Features.LeaderScripts.Commands.RunBulkReassign;
using Atlas.Application.Features.LeaderScripts.Commands.RunEndOfWeekSummary;
using Atlas.Application.Features.LeaderScripts.Commands.RunMeetingMode;
using Atlas.Application.Features.LeaderScripts.Commands.RunReleaseNoteGen;
using Atlas.Application.Features.LeaderScripts.Commands.RunSprintStarter;
using Atlas.Application.Features.LeaderScripts.Commands.RunStandupPing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class LeaderScriptsController : ApiControllerBase
{
    [HttpPost("sprint-starter")]
    public async Task<IActionResult> SprintStarter([FromBody] RunSprintStarterCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("blocked-task-blaster")]
    public async Task<IActionResult> BlockedTaskBlaster([FromBody] RunBlockedTaskBlasterCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("release-notes")]
    public async Task<IActionResult> ReleaseNoteGen([FromBody] RunReleaseNoteGenCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Notes = result });
    }

    [HttpPost("meeting-mode")]
    public async Task<IActionResult> MeetingMode([FromBody] RunMeetingModeCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }

    [HttpPost("week-summary")]
    public async Task<IActionResult> EndOfWeekSummary([FromBody] RunEndOfWeekSummaryCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("bulk-reassign")]
    public async Task<IActionResult> BulkReassign([FromBody] RunBulkReassignCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("standup-ping")]
    public async Task<IActionResult> StandupPing([FromBody] RunStandupPingCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { Output = result });
    }
}

