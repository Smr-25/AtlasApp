using Atlas.Application.Features.Integrations.Commands.ConnectIntegration;
using Atlas.Application.Features.Integrations.Commands.DeleteIntegration;
using Atlas.Application.Features.Integrations.Commands.ReconnectIntegration;
using Atlas.Application.Features.Integrations.Commands.ReportFailure;
using Atlas.Application.Features.Integrations.Commands.UpdateIntegration;
using Atlas.Application.Features.Integrations.Queries.GetIntegrationById;
using Atlas.Application.Features.Integrations.Queries.GetIntegrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class IntegrationsController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return OkResponse(await Mediator.Send(new GetIntegrationsQuery()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return OkResponse(await Mediator.Send(new GetIntegrationByIdQuery(id)));
    }

    [HttpPost]
    public async Task<IActionResult> Connect(ConnectIntegrationCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateIntegrationCommand command)
    {
        if (id != command.IntegrationId) return BadRequestResponse("Integration ID mismatch.");
        
        await Mediator.Send(command);
        return NoContentResponse();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Disconnect(Guid id)
    {
        await Mediator.Send(new DeleteIntegrationCommand(id));
        return NoContentResponse();
    }
    
    [HttpPost("{id}/reconnect")]
    public async Task<IActionResult> Reconnect(Guid id, ReconnectIntegrationCommand command)
    {
        if (id != command.IntegrationId) return BadRequestResponse("Integration ID mismatch.");
        
        await Mediator.Send(command);
        return NoContentResponse();
    }
    
    [HttpPost("{id}/mark-expired")]
    public async Task<IActionResult> MarkExpired(Guid id)
    {
        await Mediator.Send(new MarkIntegrationExpiredCommand(id));
        return NoContentResponse();
    }
}