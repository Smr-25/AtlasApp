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

    #region Success Responses

    
    protected IActionResult OkResponse<T>(T data)
    {
        return Ok(ResponseModel<T>.Success(data));
    }

    
    protected IActionResult CreatedResponse<T>(T data, string? location = null)
    {
        var response = ResponseModel<T>.Success(data);
        if (string.IsNullOrEmpty(location))
            return StatusCode(201, response);
        return Created(location, response);
    }

   
    protected IActionResult NoContentResponse()
    {
        return NoContent();
    }

    #endregion

    #region Error Responses

    protected IActionResult BadRequestResponse(string message)
    {
        return BadRequest(ResponseModel<object>.Failure(message));
    }

    protected IActionResult BadRequestResponse(IEnumerable<string> errors)
    {
        return BadRequest(ResponseModel<object>.Failure(errors));
    }

    protected IActionResult NotFoundResponse(string message)
    {
        return NotFound(ResponseModel<object>.Failure(message));
    }

  
    protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
    {
        return Unauthorized(ResponseModel<object>.Failure(message));
    }

    protected IActionResult ForbiddenResponse(string message = "Forbidden")
    {
        return StatusCode(403, ResponseModel<object>.Failure(message));
    }

    #endregion
}