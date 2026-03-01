using System.Threading.RateLimiting;
using AppSettingsMultiPlatformPackage;
using Atlas.Application;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Settings;
using Atlas.Infrastructure;
using Atlas.Persistence;
using Atlas.WebAPI.Hubs;
using Atlas.WebAPI.Middlewares;
using Atlas.WebAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddAppSettingsMultiPlatformJson(builder, "Mac");
builder.Services.AddSignalR();
builder.Services.AddScoped<IAtlasHubService, AtlasHubService>();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("TeamLeaderOnly", policy => policy.RequireRole("TeamLeader"))
    .AddPolicy("DeveloperOrSecOps", policy => policy.RequireRole("Developer", "SecOps"))
    .AddPolicy("DesignerOnly", policy => policy.RequireRole("Designer"))
    .AddPolicy("MarketerOnly", policy => policy.RequireRole("Marketer"));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                .AllowAnyMethod() 
                .AllowAnyHeader()
                .AllowCredentials();
        });
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
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AtlasHub>("/hubs/atlas");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    string[] roles = ["Developer", "Designer", "SecOps", "Marketer", "TeamLeader"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
    }
}

app.Run();