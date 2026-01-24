using System.Net;
using Atlas.Application.Exceptions.Common;
using Atlas.Application.Exceptions.Users;
using Atlas.Application.Models;
using Newtonsoft.Json;
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
            AccountLockedException accountLockedEx => (
                HttpStatusCode.Locked,
                ResponseModel<object>.Failure(accountLockedEx.Message)
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
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 32
        };
        
        var jsonResponse = JsonConvert.SerializeObject(response, jsonSettings);
        await context.Response.WriteAsync(jsonResponse);
    }
}