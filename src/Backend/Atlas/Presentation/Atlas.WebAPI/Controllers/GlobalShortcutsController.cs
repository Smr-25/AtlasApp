using Atlas.Application.Features.GlobalShortcuts.Commands.CaptureToNotion;
using Atlas.Application.Features.GlobalShortcuts.Commands.ParseCalendarEvent;
using Atlas.Application.Features.GlobalShortcuts.Commands.ProcessAiContext;
using Atlas.Application.Features.GlobalShortcuts.Commands.QuickShare;
using Atlas.Application.Features.GlobalShortcuts.Queries.SearchCommandPalette;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class GlobalShortcutsController : ApiControllerBase
{
    [HttpGet("command-palette")]
    public async Task<IActionResult> SearchCommandPalette([FromQuery] string? search)
    {
        var result = await Mediator.Send(new SearchCommandPaletteQuery(search ?? ""));
        return OkResponse(result);
    }

    [HttpPost("ai-context")]
    public async Task<IActionResult> ProcessAiContext([FromBody] ProcessAiContextCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("capture")]
    public async Task<IActionResult> CaptureToNotion([FromBody] CaptureToNotionCommand command)
    {
        var captureId = await Mediator.Send(command);
        return CreatedResponse(captureId);
    }

    [HttpPost("share")]
    public async Task<IActionResult> QuickShare([FromBody] QuickShareCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }

    [HttpPost("calendar-event")]
    public async Task<IActionResult> ParseCalendarEvent([FromBody] ParseCalendarEventCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}

