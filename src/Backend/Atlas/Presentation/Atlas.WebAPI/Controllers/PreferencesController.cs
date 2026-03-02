using Atlas.Application.Features.Preferences.Commands.UpdatePreferences;
using Atlas.Application.Features.Preferences.Queries.GetPreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class PreferencesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetPreferencesQuery());
        return OkResponse(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePreferencesCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
}

