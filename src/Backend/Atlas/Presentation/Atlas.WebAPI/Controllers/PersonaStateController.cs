using Atlas.Application.Features.PersonaStates.Commands.InitializeState;
using Atlas.Application.Features.PersonaStates.Commands.UpdateEnergyLevel;
using Atlas.Application.Features.PersonaStates.Commands.UpdateFocusLevel;
using Atlas.Application.Features.PersonaStates.Commands.UpdateMentalLoad;
using Atlas.Application.Features.PersonaStates.Commands.UpdatePhase;
using Atlas.Application.Features.PersonaStates.Queries.GetCurrentState;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonaStateController(IMediator mediator) : ControllerBase
{
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeState([FromBody] InitializeStateCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("phase")]
    public async Task<IActionResult> UpdatePhase([FromBody] UpdatePhaseCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("mental-load")]
    public async Task<IActionResult> UpdateMentalLoad([FromBody] UpdateMentalLoadCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("energy-level")]
    public async Task<IActionResult> UpdateEnergyLevel([FromBody] UpdateEnergyLevelCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("focus-level")]
    public async Task<IActionResult> UpdateFocusLevel([FromBody] UpdateFocusLevelCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentState()
    {
        var result = await mediator.Send(new GetCurrentStateQuery());
        return Ok(result);
    }
    
}