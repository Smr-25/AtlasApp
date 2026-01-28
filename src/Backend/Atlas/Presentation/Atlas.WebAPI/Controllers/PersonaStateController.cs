using Atlas.Application.Features.PersonaStates.Commands.InitializeState;
using Atlas.Application.Features.PersonaStates.Commands.UpdateEnergyLevel;
using Atlas.Application.Features.PersonaStates.Commands.UpdateFocusLevel;
using Atlas.Application.Features.PersonaStates.Commands.UpdateMentalLoad;
using Atlas.Application.Features.PersonaStates.Commands.UpdatePhase;
using Atlas.Application.Features.PersonaStates.Queries.GetCurrentState;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonaStateController(IMediator mediator) : ControllerBase
{
    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeState([FromBody] InitializeStateCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("updatePhase")]
    public async Task<IActionResult> UpdatePhase([FromBody] UpdatePhaseCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("updateMentalLoad")]
    public async Task<IActionResult> UpdateMentalLoad([FromBody] UpdateMentalLoadCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("updateEnergyLevel")]
    public async Task<IActionResult> UpdateEnergyLevel([FromBody] UpdateEnergyLevelCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("updateFocusLevel")]
    public async Task<IActionResult> UpdateFocusLevel([FromBody] UpdateFocusLevelCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpGet("currentState")]
    public async Task<IActionResult> GetCurrentState(GetCurrentStateQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }
    
}