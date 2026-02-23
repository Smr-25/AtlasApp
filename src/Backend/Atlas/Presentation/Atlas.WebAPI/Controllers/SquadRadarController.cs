using Atlas.Application.Features.SquadRadar.Commands.UpdatePresence;
using Atlas.Application.Features.SquadRadar.Queries.GetSquadRadar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class SquadRadarController : ApiControllerBase
{
    [HttpGet("{teamId:guid}")]
    public async Task<IActionResult> GetSquadRadar(Guid teamId)
    {
        var result = await Mediator.Send(new GetSquadRadarQuery(teamId));
        return OkResponse(result);
    }

    [HttpPut("presence")]
    public async Task<IActionResult> UpdatePresence([FromBody] UpdatePresenceCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }
}

