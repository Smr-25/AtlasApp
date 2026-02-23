using Atlas.Application.Features.Miro.Commands.CreateSticky;
using Atlas.Application.Features.Miro.Queries.GetBoards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class MiroController : ApiControllerBase
{
    [HttpGet("{integrationId}/boards")]
    public async Task<IActionResult> GetBoards(Guid integrationId)
    {
        var result = await Mediator.Send(new GetMiroBoardsQuery(integrationId));
        return OkResponse(result);
    }

    [HttpPost("sticky")]
    public async Task<IActionResult> CreateSticky([FromBody] CreateMiroStickyCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }
}

