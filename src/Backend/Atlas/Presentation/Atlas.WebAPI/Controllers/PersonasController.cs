using Atlas.Application.Features.Personas.Commands.AddIntegration;
using Atlas.Application.Features.Personas.Commands.CreatePersona;
using Atlas.Application.Features.Personas.Commands.DeletePersona;
using Atlas.Application.Features.Personas.Commands.SetPrimaryPersona;
using Atlas.Application.Features.Personas.Commands.UpdatePersona;
using Atlas.Application.Features.Personas.Queries.GetPersonaById;
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePersona(Guid id, [FromBody] UpdatePersonaDetailCommand command)
    {
        if (id != command.Id) return BadRequestResponse("ID mismatch");
        await Mediator.Send(command);
        return NoContentResponse();
    }
    
    [HttpPost("{id}/integrations")]
    public async Task<IActionResult> AddIntegration(Guid id, [FromBody] AddPersonaIntegrationCommand command)
    {
        if (id != command.PersonaId) 
            return BadRequestResponse("URL ID and Body ID mismatch");
        var integrationId = await Mediator.Send(command);
        return OkResponse(integrationId);
    }
    
    [HttpPost("{id}/set-primary")]
    public async Task<IActionResult> SetPrimary(Guid id)
    {
        await Mediator.Send(new SetPrimaryPersonaCommand(id));
        return NoContentResponse();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePersona(Guid id)
    {
        await Mediator.Send(new DeletePersonaCommand(id));
        return NoContentResponse();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetPersonasQuery());
        return OkResponse(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetPersonaByIdQuery(id));
        return OkResponse(result);
    }
}