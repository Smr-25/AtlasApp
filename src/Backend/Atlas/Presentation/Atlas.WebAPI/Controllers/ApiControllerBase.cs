using Atlas.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Atlas.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult OkResponse<T>(T data)
    {
        return Ok(ResponseModel<T>.Success(data));
    }

    protected IActionResult BadRequestResponse(string message)
    {
        return BadRequest(ResponseModel<object>.Failure(message));
    }
}