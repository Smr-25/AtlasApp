using Atlas.Application.Features.Workspaces.Commands.AddWorkspaceMember;
using Atlas.Application.Features.Workspaces.Commands.ChangeWorkspaceMemberRole;
using Atlas.Application.Features.Workspaces.Commands.CreateWorkspace;
using Atlas.Application.Features.Workspaces.Commands.DeleteWorkspace;
using Atlas.Application.Features.Workspaces.Commands.RemoveWorkspaceMember;
using Atlas.Application.Features.Workspaces.Commands.SetDefault;
using Atlas.Application.Features.Workspaces.Commands.ToggleIntegration;
using Atlas.Application.Features.Workspaces.Commands.UpdateWorkspace;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaceById;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaceMembers;
using Atlas.Application.Features.Workspaces.Queries.GetWorkspaces;
using Atlas.Application.Features.Workspaces.Queries.ValidateFolder;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Atlas.Application.Features.Workspaces.Dtos;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class WorkspacesController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return OkResponse(await Mediator.Send(new GetWorkspacesQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return OkResponse(await Mediator.Send(new GetWorkspaceByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateWorkspaceCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateWorkspaceCommand command)
    {
        if (id != command.WorkspaceId) return BadRequestResponse("Workspace ID mismatch.");
        
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await Mediator.Send(new DeleteWorkspaceCommand(id));
        return NoContentResponse();
    }
    
    [HttpPost("{id}/integrations/toggle")]
    public async Task<IActionResult> ToggleIntegration(Guid id, [FromBody] ToggleIntegrationDto dto)
    {
        var command = new ToggleWorkspaceIntegrationCommand(id, dto.IntegrationId, dto.Enable);
        await Mediator.Send(command);
        return NoContentResponse();
    }
    
    [HttpPatch("{id}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        await Mediator.Send(new SetDefaultWorkspaceCommand(id));
        return NoContentResponse();
    }

    [HttpPost("validate-folder")]
    public async Task<IActionResult> ValidateFolder([FromBody] ValidateFolderRequest request)
    {
        var result = await Mediator.Send(new ValidateFolderQuery(request.FolderPath));
        return OkResponse(result);
    }
    
    // ─── Member Management ─────────────────────────────────────
    
    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(Guid id)
    {
        return OkResponse(await Mediator.Send(new GetWorkspaceMembersQuery(id)));
    }
    
    [HttpPost("{id}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddWorkspaceMemberRequest request)
    {
        await Mediator.Send(new AddWorkspaceMemberCommand(id, request.UserId, request.Role));
        return NoContentResponse();
    }
    
    [HttpDelete("{id}/members/{userId}")]
    public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
    {
        await Mediator.Send(new RemoveWorkspaceMemberCommand(id, userId));
        return NoContentResponse();
    }
    
    [HttpPatch("{id}/members/{userId}/role")]
    public async Task<IActionResult> ChangeMemberRole(Guid id, Guid userId, [FromBody] ChangeRoleRequest request)
    {
        await Mediator.Send(new ChangeWorkspaceMemberRoleCommand(id, userId, request.NewRole));
        return NoContentResponse();
    }
}

public record ValidateFolderRequest(string FolderPath);
public record AddWorkspaceMemberRequest(Guid UserId, WorkspaceMemberRole Role = WorkspaceMemberRole.Viewer);
public record ChangeRoleRequest(WorkspaceMemberRole NewRole);
