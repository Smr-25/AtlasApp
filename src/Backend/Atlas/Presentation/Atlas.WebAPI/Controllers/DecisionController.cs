using Atlas.Application.Features.Decisions.Commands.AbandonDecision;
using Atlas.Application.Features.Decisions.Commands.CreateDecision;
using Atlas.Application.Features.Decisions.Commands.ExecuteDecision;
using Atlas.Application.Features.Decisions.Commands.PostponeDecision;
using Atlas.Application.Features.Decisions.Commands.RecordOutcome;
using Atlas.Application.Features.Decisions.Commands.UpdateDecision;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DecisionController(IMediator mediator) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateDecision([FromBody] CreateDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateDecision([FromBody] UpdateDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteDecision([FromBody] ExecuteDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("abandon")]
    public async Task<IActionResult> AbandonDecision([FromBody] AbandonDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("postpone")]
    public async Task<IActionResult> PostponeDecision([FromBody] PostponeDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("outcome")]
    public async Task<IActionResult> RecordOutcome([FromBody] RecordOutcomeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
