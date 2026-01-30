using Atlas.Application.Features.Decisions.Commands.AbandonDecision;
using Atlas.Application.Features.Decisions.Commands.CreateDecision;
using Atlas.Application.Features.Decisions.Commands.ExecuteDecision;
using Atlas.Application.Features.Decisions.Commands.PostponeDecision;
using Atlas.Application.Features.Decisions.Commands.RecordOutcome;
using Atlas.Application.Features.Decisions.Commands.UpdateDecision;
using Atlas.Application.Features.Decisions.Queires.GetDecisionWithContext;
using Atlas.Application.Features.Decisions.Queries.GetDecisionById;
using Atlas.Application.Features.Decisions.Queries.GetDecisionsByStatus;
using Atlas.Application.Features.Decisions.Queries.GetMyDecisions;
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

    [HttpPut("update")]
    public async Task<IActionResult> UpdateDecisionPut([FromBody] UpdateDecisionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDecisionById()
    {
        var result = await mediator.Send(new GetDecisionByIdQuery(id));
        return Ok(result);
    }

    [HttpGet("{id}/detail")]
    public async Task<IActionResult> GetDecisionWithContext()
    {
        var result = await mediator.Send(new GetDecisionWithContextQuery());
        return Ok(result);
    }

    [HttpGet("decisions")]
    public async Task<IActionResult> GetMyDecisions()
    {
        var result = await mediator.Send(new GetMyDecisionsQuery());
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetDecisionsByStatus()
    {
        var result = await mediator.Send(new GetDecisionsByStatusQuery());
        return Ok(result);
    }
}