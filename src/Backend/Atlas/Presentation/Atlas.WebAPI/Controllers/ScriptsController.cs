using Atlas.Application.Features.Scripts.Commands.CreateScript;
using Atlas.Application.Features.Scripts.Commands.RunScript;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ScriptsController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateScriptCommand command)
    {
        return await Mediator.Send(command);
    }
    
    [HttpPost("{id}/run")]
    public async Task<ActionResult<string>> Run(Guid id)
    {
        var result = await Mediator.Send(new RunScriptCommand(id));
        return Ok(new { Output = result });
    }
}