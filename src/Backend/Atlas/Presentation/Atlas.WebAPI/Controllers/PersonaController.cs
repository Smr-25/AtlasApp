using Atlas.Application.Features.Personas.Commands.ActivePersona;
using Atlas.Application.Features.Personas.Commands.CreatePersona;
using Atlas.Application.Features.Personas.Commands.DeactivePersona;
using Atlas.Application.Features.Personas.Commands.UpdatePersona;
using Atlas.Application.Features.Personas.Queries.CheckPersonaExists;
using Atlas.Application.Features.Personas.Queries.GetMyPersona;
using Atlas.Application.Features.Personas.Queries.GetPersonaById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonaController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreatePersona(CreatePersonaCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdatePersona(UpdatePersonaCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("active")]
    public async Task<IActionResult> ActivePersona(ActivePersonaCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("deactive")]
    public async Task<IActionResult> DeactivePersona(DeactivePersonaCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("exists")]
    public async Task<IActionResult> CheckPersonaExists(CheckPersonaExistsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("my-persona")]
    public async Task<IActionResult> GetMyPersona(GetMyPersonaQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("my-persona-Id")]
    public async Task<IActionResult> GetMyPersonaId(GetPersonaByIdQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.Data?.Id);
    }
}