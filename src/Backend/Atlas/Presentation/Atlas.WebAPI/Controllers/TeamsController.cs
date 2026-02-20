using Atlas.Application.Features.Teams.Commands.CreateTeam;
using Atlas.Application.Features.Teams.Commands.InviteMember;
using Atlas.Application.Features.Teams.Commands.RemoveMember;
using Atlas.Application.Features.Teams.Queries.GetTeamDashboard;
using Atlas.Application.Features.Teams.Queries.GetTeamRadar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class TeamsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTeamCommand command)
    {
        var teamId = await Mediator.Send(command);
        return CreatedResponse(teamId);
    }

    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetDashboard(Guid teamId)
    {
        var result = await Mediator.Send(new GetTeamDashboardQuery(teamId));
        return OkResponse(result);
    }

    [HttpPost("{teamId}/members")]
    public async Task<IActionResult> InviteMember(Guid teamId, [FromBody] InviteMemberRequest request)
    {
        var result = await Mediator.Send(new InviteMemberCommand(teamId, request.UserId));
        return OkResponse(result);
    }

    [HttpDelete("{teamId}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId)
    {
        await Mediator.Send(new RemoveMemberCommand(teamId, userId));
        return NoContentResponse();
    }

    [HttpGet("{teamId}/radar")]
    [Authorize(Policy = "TeamLeaderOnly")]
    public async Task<IActionResult> GetRadar(Guid teamId)
    {
        var result = await Mediator.Send(new GetTeamRadarQuery(teamId));
        return OkResponse(result);
    }
}

public record InviteMemberRequest(Guid UserId);

