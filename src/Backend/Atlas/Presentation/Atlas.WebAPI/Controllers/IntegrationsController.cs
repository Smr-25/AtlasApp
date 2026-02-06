using Atlas.Application.Features.Integrations.Commands.ConnectIntegration;
using Atlas.Application.Features.Integrations.Commands.DeleteIntegration;
using Atlas.Application.Features.Integrations.Commands.UpdateIntegration;
using Atlas.Application.Features.Integrations.Dtos;
using Atlas.Application.Features.Integrations.Queries.GetIntegrationById;
using Atlas.Application.Features.Integrations.Queries.GetIntegrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class IntegrationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<IntegrationDto>>> GetAll()
    {
        return Ok(await Mediator.Send(new GetIntegrationsQuery()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IntegrationDto>> GetById(Guid id)
    {
        return Ok(await Mediator.Send(new GetIntegrationByIdQuery(id)));
    }

    [HttpPost]
    public async Task<ActionResult<IntegrationDto>> Connect(ConnectIntegrationCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateIntegrationCommand command)
    {
        if (id != command.IntegrationId) return BadRequest();
        
        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Disconnect(Guid id)
    {
        await Mediator.Send(new DeleteIntegrationCommand(id));
        return NoContent();
    }
}