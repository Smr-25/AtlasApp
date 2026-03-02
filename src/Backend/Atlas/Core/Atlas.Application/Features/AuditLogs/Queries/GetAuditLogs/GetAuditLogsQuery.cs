using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.AuditLogs.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.AuditLogs.Queries.GetAuditLogs;

public record GetAuditLogsQuery(
    AuditAction? Action = null,
    DateTime? From = null,
    DateTime? To = null,
    int Page = 1,
    int PageSize = 50
) : IRequest<List<AuditLogDto>>;

public class GetAuditLogsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetAuditLogsQuery, List<AuditLogDto>>
{
    public async Task<List<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var query = context.AuditLogs.Where(a => a.UserId == userId).AsQueryable();

        if (request.Action.HasValue)
            query = query.Where(a => a.Action == request.Action.Value);
        if (request.From.HasValue)
            query = query.Where(a => a.CreatedAt >= new DateTimeOffset(request.From.Value, TimeSpan.Zero));
        if (request.To.HasValue)
            query = query.Where(a => a.CreatedAt <= new DateTimeOffset(request.To.Value, TimeSpan.Zero));

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AuditLogDto(
                a.Id, a.Action, a.Description, a.EntityName, a.EntityId,
                a.IpAddress, a.UserAgent, a.WorkspaceId, a.CreatedAt))
            .ToListAsync(ct);
    }
}

