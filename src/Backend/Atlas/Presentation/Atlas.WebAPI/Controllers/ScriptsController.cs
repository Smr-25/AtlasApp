using Atlas.Application.Features.Scripts.Commands.CreateScript;
using Atlas.Application.Features.Scripts.Commands.RunScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ScriptsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScriptCommand command)
    {
        var scriptId = await Mediator.Send(command);
        return CreatedResponse(scriptId);
    }
    
    [HttpPost("{id}/run")]
    public async Task<IActionResult> Run(Guid id)
    {
        var result = await Mediator.Send(new RunScriptCommand(id));
        return OkResponse(new { Output = result });
    }
}