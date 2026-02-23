using Atlas.Application.Features.ResourceHub.Commands.AddResource;
using Atlas.Application.Features.ResourceHub.Commands.DeleteResource;
using Atlas.Application.Features.ResourceHub.Commands.PinResource;
using Atlas.Application.Features.ResourceHub.Commands.UpdateResource;
using Atlas.Application.Features.ResourceHub.Queries.GetResources;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class ResourceHubController : ApiControllerBase
{
    [HttpGet("{teamId:guid}")]
    public async Task<IActionResult> GetResources(Guid teamId, [FromQuery] ResourceCategory? category)
    {
        var result = await Mediator.Send(new GetResourcesQuery(teamId, category));
        return OkResponse(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddResource([FromBody] AddResourceCommand command)
    {
        var id = await Mediator.Send(command);
        return CreatedResponse(new { Id = id });
    }

    [HttpPut]
    public async Task<IActionResult> UpdateResource([FromBody] UpdateResourceCommand command)
    {
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpDelete("{resourceId:guid}")]
    public async Task<IActionResult> DeleteResource(Guid resourceId)
    {
        await Mediator.Send(new DeleteResourceCommand(resourceId));
        return NoContentResponse();
    }

    [HttpPost("{resourceId:guid}/pin")]
    public async Task<IActionResult> PinResource(Guid resourceId)
    {
        await Mediator.Send(new PinResourceCommand(resourceId));
        return NoContentResponse();
    }
}

