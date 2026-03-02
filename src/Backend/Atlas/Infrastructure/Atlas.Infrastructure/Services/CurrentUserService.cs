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
    public string? Language => httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString().Split(',').FirstOrDefault();
    public int TimezoneOffsetInMinutes => int.TryParse(
        httpContextAccessor.HttpContext?.Request.Headers["X-Timezone-Offset"].FirstOrDefault(),
        out var offset) ? offset : 0;
    
    public Guid? WorkspaceId
    {
        get
        {
            var header = httpContextAccessor.HttpContext?.Request.Headers["X-Workspace-Id"].FirstOrDefault();
            return Guid.TryParse(header, out var workspaceId) ? workspaceId : null;
        }
    }
}