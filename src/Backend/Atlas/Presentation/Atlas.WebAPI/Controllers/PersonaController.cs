using Atlas.Application.Features.Personas.Commands.AddIntegration;
using Atlas.Application.Features.Personas.Commands.CreatePersona;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

public class PersonaController : ApiControllerBase
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
}