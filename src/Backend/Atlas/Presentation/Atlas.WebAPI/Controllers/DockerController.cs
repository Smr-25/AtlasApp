using Atlas.Application.Features.Docker.Commands.ControlContainer;
using Atlas.Application.Features.Docker.Dtos;
using Atlas.Application.Features.Docker.Queries.GetContainerLogs;
using Atlas.Application.Features.Docker.Queries.GetContainers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class DockerController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ContainerDto>>> GetContainers()
    {
        return await Mediator.Send(new GetContainersQuery());
    }

    [HttpGet("{id}/logs")]
    public async Task<ActionResult<string>> GetLogs(string id)
    {
        return await Mediator.Send(new GetContainerLogsQuery(id));
    }
    
    [HttpPost("{id}/start")]
    public async Task<ActionResult> Start(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Start));
        return NoContent();
    }

    [HttpPost("{id}/stop")]
    public async Task<ActionResult> Stop(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Stop));
        return NoContent();
    }
    
    [HttpPost("{id}/restart")]
    public async Task<ActionResult> Restart(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Restart));
        return NoContent();
    }
}