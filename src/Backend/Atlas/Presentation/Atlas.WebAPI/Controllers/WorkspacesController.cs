using Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspacesByPersona;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class WorkspacesController : ApiControllerBase
{
    [HttpGet("bypersona/{personaId}")]
    public async Task<IActionResult> GetByPersona(Guid personaId)
    {
        var result = await Mediator.Send(new GetWorkspacesByPersonaQuery(personaId));
        return OkResponse(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkspaceCommand command)
    {
        var workspaceId = await Mediator.Send(command);
        return CreatedResponse(workspaceId);
    }
}