using System.Threading.RateLimiting;
using AppSettingsMultiPlatformPackage;
using Atlas.Application;
using Atlas.Application.Settings;
using Atlas.Infrastructure;
using Atlas.Persistence;
using Atlas.WebAPI.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
builder.Services.AddOpenApi();
builder.Services.AddAppSettingsMultiPlatformJson(builder, "Mac");
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

var rateLimitSettings = builder.Configuration.GetSection("RateLimitSettings").Get<RateLimitSettings>()
                        ?? new RateLimitSettings();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Fixed.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Fixed.WindowInSeconds);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("login", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Login.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Login.WindowInSeconds);
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("register", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Register.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Register.WindowInSeconds);
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("password-reset", opt =>
    {
        opt.PermitLimit = rateLimitSettings.PasswordReset.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.PasswordReset.WindowInSeconds);
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("verification", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Verification.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Verification.WindowInSeconds);
        opt.QueueLimit = 0;
    });

    options.AddFixedWindowLimiter("resend", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Resend.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Resend.WindowInSeconds);
        opt.QueueLimit = 0;
    });

    options.AddSlidingWindowLimiter("api", opt =>
    {
        opt.PermitLimit = rateLimitSettings.Api.PermitLimit;
        opt.Window = TimeSpan.FromSeconds(rateLimitSettings.Api.WindowInSeconds);
        opt.SegmentsPerWindow = rateLimitSettings.Api.SegmentsPerWindow;
        opt.QueueLimit = 0;
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            errors = new[] { "Too many requests. Please try again later." }
        };

        await context.HttpContext.Response.WriteAsJsonAsync(response, token);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();