using System.Net;
using System.Text.Json;
using Atlas.Application.Exceptions.Common;
using Atlas.Application.Exceptions.Users;
using Atlas.Application.Models;
using ValidationException = Atlas.Application.Exceptions.Common.ValidationException;

namespace Atlas.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var (statusCode, response) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(validationEx.Errors.SelectMany(e => e.Value))
            ),
            BadRequestException badRequestEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(badRequestEx.Message)
            ),
            InvalidOrExpiredCodeException codeEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(codeEx.Message)
            ),
            AlreadyVerifiedException verifiedEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(verifiedEx.Message)
            ),
            InvalidVerificationChannelException channelEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(channelEx.Message)
            ),
            IdentityException identityEx => (
                HttpStatusCode.BadRequest,
                ResponseModel<object>.Failure(identityEx.Errors)
            ),
            
            UnauthorizedException unauthorizedEx => (
                HttpStatusCode.Unauthorized,
                ResponseModel<object>.Failure(unauthorizedEx.Message)
            ),
            InvalidCredentialsException credentialsEx => (
                HttpStatusCode.Unauthorized,
                ResponseModel<object>.Failure(credentialsEx.Message)
            ),
            EmailNotVerifiedException emailNotVerifiedEx => (
                HttpStatusCode.Unauthorized,
                ResponseModel<object>.Failure(emailNotVerifiedEx.Message)
            ),
            
            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                ResponseModel<object>.Failure(forbiddenEx.Message)
            ),
            
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                ResponseModel<object>.Failure(notFoundEx.Message)
            ),
            
            AlreadyExistException alreadyExistEx => (
                HttpStatusCode.Conflict,
                ResponseModel<object>.Failure(alreadyExistEx.Message)
            ),
            
            _ => (
                HttpStatusCode.InternalServerError,
                ResponseModel<object>.Failure("An internal server error occurred.")
            )
        };

        context.Response.StatusCode = (int)statusCode;
        
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var serialized = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(serialized);
    }
}