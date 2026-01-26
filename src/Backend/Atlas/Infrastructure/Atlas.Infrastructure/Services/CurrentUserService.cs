using System.Security.Claims;
using Atlas.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Atlas.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?
        .FindFirstValue(ClaimTypes.NameIdentifier);

    public string? Email => httpContextAccessor.HttpContext?.User?
        .FindFirstValue(ClaimTypes.Email);

    public string? UserName => httpContextAccessor.HttpContext?.User?
        .FindFirstValue(ClaimTypes.Name);

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}