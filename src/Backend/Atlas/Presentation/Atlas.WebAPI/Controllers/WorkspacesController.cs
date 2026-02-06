using Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;
using Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;
using Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;
using Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Atlas.Application.Features.Workspaces.Dtos;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class WorkspacesController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<WorkspaceDto>>> GetAll()
    {
        return Ok(await Mediator.Send(new GetWorkspacesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkspaceDto>> GetById(Guid id)
    {
        return Ok(await Mediator.Send(new GetWorkspaceByIdQuery(id)));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateWorkspaceCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateWorkspaceCommand command)
    {
        if (id != command.WorkspaceId) return BadRequest();
        
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteWorkspaceCommand(id));
        return NoContent();
    }
    
    [HttpPost("{id}/integrations/toggle")]
    public async Task<IActionResult> ToggleIntegration(Guid id, [FromBody] ToggleIntegrationDto dto)
    {
        // Route-dan gələn ID-ni command-a ötürürük
        var command = new ToggleWorkspaceIntegrationCommand(id, dto.IntegrationId, dto.Enable);
        
        await Mediator.Send(command);
        return NoContent();
    }
}