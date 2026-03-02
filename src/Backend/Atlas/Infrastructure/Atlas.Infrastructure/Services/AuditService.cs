using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Atlas.Infrastructure.Services;

public class AuditService(
    IApplicationDbContext context,
    IHttpContextAccessor httpContextAccessor) : IAuditService
{
    public async Task LogAsync(
        Guid userId,
        AuditAction action,
        string description,
        string? entityName = null,
        Guid? entityId = null,
        string? metadataJson = null,
        Guid? workspaceId = null,
        CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();

        var log = AuditLog.Create(
            userId, action, description,
            entityName, entityId,
            ipAddress, userAgent,
            metadataJson, workspaceId);

        await context.AuditLogs.AddAsync(log, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

