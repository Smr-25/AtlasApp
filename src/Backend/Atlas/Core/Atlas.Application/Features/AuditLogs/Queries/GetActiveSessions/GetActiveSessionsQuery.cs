using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.AuditLogs.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.AuditLogs.Queries.GetActiveSessions;

public record GetActiveSessionsQuery : IRequest<List<ActiveSessionDto>>;

public class GetActiveSessionsQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetActiveSessionsQuery, List<ActiveSessionDto>>
{
    public async Task<List<ActiveSessionDto>> Handle(GetActiveSessionsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var sessions = await context.AuditLogs
            .Where(a => a.UserId == userId && a.Action == AuditAction.Login)
            .OrderByDescending(a => a.CreatedAt)
            .Take(20)
            .Select(a => new ActiveSessionDto(
                a.IpAddress ?? "Unknown",
                a.UserAgent ?? "Unknown",
                a.CreatedAt.UtcDateTime,
                false))
            .ToListAsync(ct);

        if (sessions.Count > 0)
            sessions[0] = sessions[0] with { IsCurrent = true };

        return sessions;
    }
}
