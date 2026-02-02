using Atlas.Application.Features.Integrations.Commands.AddIntegration;
using Atlas.Application.Features.Integrations.Queries.GetIntegrationResources;
using Atlas.Application.Features.Integrations.Queries.GetIntegrationsByPersona;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class IntegrationsController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddIntegration([FromBody] AddIntegrationCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResponse(new { IntegrationId = result });
    }
    
    [HttpGet("persona/{personaId}")]
    public async Task<IActionResult> GetByPersona(Guid personaId)
    {
        var result = await Mediator.Send(new GetIntegrationsByPersonaQuery(personaId));
        return OkResponse(result);
    }
    
    [HttpGet("{id}/resources")]
    public async Task<IActionResult> GetResources(Guid id)
    {
        var result = await Mediator.Send(new GetIntegrationResourcesQuery(id));
        return OkResponse(result);
    }
}