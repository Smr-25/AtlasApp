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
    public async Task<IActionResult> GetContainers()
    {
        var result = await Mediator.Send(new GetContainersQuery());
        return OkResponse(result);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogs(string id)
    {
        var result = await Mediator.Send(new GetContainerLogsQuery(id));
        return OkResponse(result);
    }
    
    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Start));
        return NoContentResponse();
    }

    [HttpPost("{id}/stop")]
    public async Task<IActionResult> Stop(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Stop));
        return NoContentResponse();
    }
    
    [HttpPost("{id}/restart")]
    public async Task<IActionResult> Restart(string id)
    {
        await Mediator.Send(new ControlContainerCommand(id, DockerAction.Restart));
        return NoContentResponse();
    }
}