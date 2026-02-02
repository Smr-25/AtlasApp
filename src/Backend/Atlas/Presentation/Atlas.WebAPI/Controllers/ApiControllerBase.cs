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

    /// <summary>
    /// Returns 400 Bad Request with error message.
    /// </summary>
    protected IActionResult BadRequestResponse(string message)
    {
        return BadRequest(ResponseModel<object>.Failure(message));
    }

    /// <summary>
    /// Returns 400 Bad Request with multiple error messages.
    /// </summary>
    protected IActionResult BadRequestResponse(IEnumerable<string> errors)
    {
        return BadRequest(ResponseModel<object>.Failure(errors));
    }

    /// <summary>
    /// Returns 404 Not Found with error message.
    /// </summary>
    protected IActionResult NotFoundResponse(string message)
    {
        return NotFound(ResponseModel<object>.Failure(message));
    }

    /// <summary>
    /// Returns 401 Unauthorized with error message.
    /// </summary>
    protected IActionResult UnauthorizedResponse(string message = "Unauthorized")
    {
        return Unauthorized(ResponseModel<object>.Failure(message));
    }

    /// <summary>
    /// Returns 403 Forbidden with error message.
    /// </summary>
    protected IActionResult ForbiddenResponse(string message = "Forbidden")
    {
        return StatusCode(403, ResponseModel<object>.Failure(message));
    }

    #endregion
}