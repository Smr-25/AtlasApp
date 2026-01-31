using Atlas.Application.Features.Constraints.Commands.CreateConstraint;
using Atlas.Application.Features.Constraints.Commands.DeactivateConstraint;
using Atlas.Application.Features.Constraints.Commands.UpdateConstraint;
using Atlas.Application.Features.Constraints.Queries.GetActiveConstraints;
using Atlas.Application.Features.Constraints.Queries.GetConstraintById;
using Atlas.Application.Features.Constraints.Queries.GetMyConstraints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConstraintController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateConstraint([FromBody] CreateConstraintCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetConstraintById([FromRoute] Guid id)
    {
        var result = await mediator.Send(new GetConstraintByIdQuery());
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveConstraints()
    {
        var result = await mediator.Send(new GetActiveConstraintsQuery());
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyConstraints()
    {
        var result = await mediator.Send(new GetMyConstraintsQuery());
        return Ok();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateConstraint(UpdateConstraintCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateConstraint(DeactivateConstraintCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}