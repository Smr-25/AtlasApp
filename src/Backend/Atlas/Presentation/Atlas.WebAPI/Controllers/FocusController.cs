using Atlas.Application.Features.Focus.Commands.LogSession;
using Atlas.Application.Features.Focus.Dtos;
using Atlas.Application.Features.Focus.Queries.GetFocusStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class FocusController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> LogSession(LogSessionCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpGet("stats")]
    public async Task<ActionResult<FocusStatsDto>> GetStats()
    {
        return await Mediator.Send(new GetFocusStatsQuery());
    }
}