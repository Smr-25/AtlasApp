using Atlas.Application.Features.Focus.Commands.CompleteFocusSession;
using Atlas.Application.Features.Focus.Commands.InterruptFocusSession;
using Atlas.Application.Features.Focus.Commands.LogSession;
using Atlas.Application.Features.Focus.Commands.PauseFocusSession;
using Atlas.Application.Features.Focus.Dtos;
using Atlas.Application.Features.Focus.Queries.GetFocusHistory;
using Atlas.Application.Features.Focus.Queries.GetFocusStats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class FocusController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> LogSession(LogSessionCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await Mediator.Send(new GetFocusStatsQuery());
        return OkResponse(result);
    }

    [HttpPost("{sessionId}/complete")]
    public async Task<IActionResult> Complete(Guid sessionId)
    {
        var result = await Mediator.Send(new CompleteFocusSessionCommand(sessionId));
        return OkResponse(result);
    }

    [HttpPost("{sessionId}/pause")]
    public async Task<IActionResult> Pause(Guid sessionId)
    {
        var result = await Mediator.Send(new PauseFocusSessionCommand(sessionId));
        return OkResponse(result);
    }

    [HttpPost("{sessionId}/interrupt")]
    public async Task<IActionResult> Interrupt(Guid sessionId)
    {
        var result = await Mediator.Send(new InterruptFocusSessionCommand(sessionId));
        return OkResponse(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] int days = 7)
    {
        var result = await Mediator.Send(new GetFocusHistoryQuery(days));
        return OkResponse(result);
    }
}