using Atlas.Application.Features.Personas.Commands.AddIntegration;
using Atlas.Application.Features.Personas.Commands.CreatePersona;
using Atlas.Application.Features.Personas.Queries.GetPersonas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[Authorize]
public class PersonasController : ApiControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePersona([FromBody] CreatePersonaCommand command)
    {
        var personaId = await Mediator.Send(command);
        return OkResponse(personaId);
    }

    [HttpPost("{id}/integrations")]
    public async Task<IActionResult> AddIntegration(Guid id,[FromBody] AddIntegrationCommand command)
    {
        if (id != command.PersonaId) 
            return BadRequestResponse("URL ID and Body ID mismatch");
        var integrationId = await Mediator.Send(command);
        return OkResponse(integrationId);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetPersonasQuery());
        return OkResponse(result);
    }
}