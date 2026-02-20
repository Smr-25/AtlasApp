using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Focus.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Queries.GetFocusHistory;

public class GetFocusHistoryQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetFocusHistoryQuery, List<FocusHistoryDto>>
{
    public async Task<List<FocusHistoryDto>> Handle(GetFocusHistoryQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        var since = DateTime.UtcNow.AddDays(-request.Days);

        var sessions = await dbContext.FocusSessions
            .Where(fs => fs.UserId == userId && fs.CreatedAt >= since)
            .OrderByDescending(fs => fs.CreatedAt)
            .ToListAsync(cancellationToken);

        return sessions.Select(s => new FocusHistoryDto(
            s.Id,
            s.DurationMinutes,
            s.BreakDurationMinutes,
            s.Tag,
            s.SessionType.ToString(),
            s.Status.ToString(),
            s.StartedAt,
            s.CompletedAt,
            s.WorkspaceId
        )).ToList();
    }
}

