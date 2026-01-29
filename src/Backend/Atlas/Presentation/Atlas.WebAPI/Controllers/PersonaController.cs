using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Commands.ActivatePersona;
using Atlas.Application.Features.Personas.Commands.CreatePersona;
using Atlas.Application.Features.Personas.Commands.DeactivatePersona;
using Atlas.Application.Features.Personas.Commands.UpdatePersona;
using Atlas.Application.Features.Personas.Dtos;
using Atlas.Application.Features.Personas.Queries.CheckPersonaExists;
using Atlas.Application.Features.Personas.Queries.GetMyPersona;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonaController(IMediator mediator) : ControllerBase
{
    #region Commands

    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<PersonaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreatePersonaCommand command)
        => Ok(await mediator.Send(command));

    [HttpPut]
    [ProducesResponseType(typeof(ResponseModel<PersonaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] UpdatePersonaCommand command)
        => Ok(await mediator.Send(command));

    [HttpPatch("activate")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Activate()
        => Ok(await mediator.Send(new ActivatePersonaCommand()));

    [HttpPatch("deactivate")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Deactivate()
        => Ok(await mediator.Send(new DeactivatePersonaCommand()));

    #endregion

    #region Queries

    [HttpGet("me")]
    [ProducesResponseType(typeof(ResponseModel<PersonaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe()
        => Ok(await mediator.Send(new GetMyPersonaQuery()));

    [HttpGet("exists")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Exists()
        => Ok(await mediator.Send(new CheckPersonaExistsQuery()));

    #endregion
}