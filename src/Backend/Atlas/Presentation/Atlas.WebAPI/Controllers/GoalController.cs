using Atlas.Application.Features.Goals.Commands.CompleteGoal;
using Atlas.Application.Features.Goals.Commands.CreateGoal;
using Atlas.Application.Features.Goals.Commands.UpdateGoal;
using Atlas.Application.Features.Goals.Commands.UpdateGoalProgress;
using Atlas.Application.Features.Goals.Queries.GetGoalById;
using Atlas.Application.Features.Goals.Queries.GetGoalsByStatus;
using Atlas.Application.Features.Goals.Queries.GetMyGoals;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GoalController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGoal([FromBody] CreateGoalCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGoalById([FromRoute] Guid id)
    {
        var result = await mediator.Send(new GetGoalByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyGoals()
    {
        var result = await mediator.Send(new GetMyGoalsQuery());
        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetGoalsByStatus([FromRoute] string status)
    {
        var result = await mediator.Send(new GetGoalsByStatusQuery());
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGoal([FromRoute] Guid id, [FromBody] UpdateGoalCommand command)
    {
        if (id != command.GoalId)
            return BadRequest("Goal ID mismatch.");
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}/progress")]
    public async Task<IActionResult> UpdateGoalProgress([FromRoute] Guid id,
        [FromBody] UpdateGoalProgressCommand command)
    {
        if (id != command.GoalId)
            return BadRequest("Goal ID mismatch.");
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> CompleteGoal([FromRoute] Guid id,
        [FromBody] CompleteGoalCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/pause")]
    public async Task<IActionResult> PauseGoal([FromRoute] Guid id,
        [FromBody] CompleteGoalCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/resume")]
    public async Task<IActionResult> ResumeGoal([FromRoute] Guid id,
        [FromBody] CompleteGoalCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/abandon")]
    public async Task<IActionResult> AbandonGoal([FromRoute] Guid id,
        [FromBody] CompleteGoalCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}