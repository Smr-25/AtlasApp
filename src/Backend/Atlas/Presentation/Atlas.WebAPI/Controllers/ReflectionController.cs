using Atlas.Application.Features.Reflections.Commands.CreateReflection;
using Atlas.Application.Features.Reflections.Commands.DeleteReflection;
using Atlas.Application.Features.Reflections.Commands.UpdateReflection;
using Atlas.Application.Features.Reflections.Queries.GetMyReflections;
using Atlas.Application.Features.Reflections.Queries.GetReflectionsByType;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReflectionController(IMediator mediator) : ControllerBase
{
    // POST   /api/reflections              → CreateReflection
    //     GET    /api/reflections/{id}         → GetReflectionById
    //     GET    /api/reflections              → GetMyReflections
    //     GET    /api/reflections/type/{type}  → GetReflectionsByType
    //     PUT    /api/reflections/{id}         → UpdateReflection
    //     DELETE /api/reflections/{id}         → DeleteReflection

    [HttpGet]
    public async Task<IActionResult> GetMyReflections()
    {
        var result = await mediator.Send(new GetReflectionsQuery());
        return Ok(result);
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetReflectionsByType()
    {
        var result = await mediator.Send(new GetReflectionsByTypeQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReflectionById()
    {
        var result = await mediator.Send(new GetReflectionsByTypeQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReflection(CreateReflectionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReflection(UpdateReflectionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReflection(DeleteReflectionCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}