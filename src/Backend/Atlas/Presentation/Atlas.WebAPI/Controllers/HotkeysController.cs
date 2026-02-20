using Atlas.Application.Features.Hotkeys.Commands.DeleteHotkey;
using Atlas.Application.Features.Hotkeys.Commands.SeedDefaultHotkeys;
using Atlas.Application.Features.Hotkeys.Commands.SetHotkey;
using Atlas.Application.Features.Hotkeys.Queries.GetMyHotkeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class HotkeysController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetMyHotkeysQuery());
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> Set([FromBody] SetHotkeyCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpDelete("{hotkeyId}")]
    public async Task<IActionResult> Delete(Guid hotkeyId)
    {
        var result = await Mediator.Send(new DeleteHotkeyCommand(hotkeyId));
        return OkResponse(result);
    }

    [HttpPost("seed-defaults")]
    public async Task<IActionResult> SeedDefaults()
    {
        var count = await Mediator.Send(new SeedDefaultHotkeysCommand());
        return OkResponse(new { CreatedCount = count });
    }
}

