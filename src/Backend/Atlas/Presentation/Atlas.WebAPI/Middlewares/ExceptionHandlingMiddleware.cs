using System.Text.Json;
using Atlas.Application.Models;
using Microsoft.AspNetCore.Http;

namespace Atlas.WebAPI.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var responseModel = ResponseModel<bool>.Failure(exception.Message);
        context.Response.ContentType = "application/json";
        var serialized = JsonSerializer.Serialize(responseModel);
        await context.Response.WriteAsync(serialized);
    }
}