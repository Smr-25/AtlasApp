using Atlas.Application.Features.Workspaces.Commands.ChatWithWorkspace;
using Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;
using Atlas.Application.Features.Workspaces.Commands.LinkIntegration;
using Atlas.Application.Features.Workspaces.Commands.UpdateLinkConfig;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspacesByPersona;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaceTools;
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
    
    [HttpPost("link-tool")]
    public async Task<IActionResult> LinkTool([FromBody] LinkIntegrationCommand command)
    {
        await Mediator.Send(command);
        return OkResponse("Integration linked successfully.");
    }
    
    [HttpGet("{id}/tools")]
    public async Task<IActionResult> GetTools(Guid id)
    {
        var result = await Mediator.Send(new GetWorkspaceToolsQuery(id));
        return OkResponse(result);
    }
    
    [HttpPut("link-config")]
    public async Task<IActionResult> UpdateLinkConfig([FromBody] UpdateLinkConfigCommand command)
    {
        await Mediator.Send(command);
        return OkResponse("Configuration updated.");
    }
    
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatWithWorkspaceCommand command)
    {
        var response = await Mediator.Send(command);
        return OkResponse(new { Response = response });
    }
}