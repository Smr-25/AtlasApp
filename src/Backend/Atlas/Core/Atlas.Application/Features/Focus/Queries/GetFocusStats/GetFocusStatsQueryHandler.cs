using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Focus.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Queries.GetFocusStats;

public class GetFocusStatsQueryHandler(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
    : IRequestHandler<GetFocusStatsQuery, FocusStatsDto>
{
    public async Task<FocusStatsDto> Handle(GetFocusStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUserService.UserId ?? Guid.Empty.ToString());
        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        var completedSessions = await applicationDbContext.FocusSessions
            .Where(s => s.UserId == userId && s.Status == FocusSessionStatus.Completed && s.CompletedAt >= weekStart)
            .ToListAsync(cancellationToken);

        var todaysSessions = completedSessions.Where(s => s.CompletedAt >= today).ToList();
        var weekSessions = completedSessions;

        var todayCount = todaysSessions.Count;
        var todayMinutes = todaysSessions.Sum(s => s.DurationMinutes);
        var weekCount = weekSessions.Count;
        var weekMinutes = weekSessions.Sum(s => s.DurationMinutes);

        var streak = todayCount > 0 ? 1 : 0;

        return new FocusStatsDto(todayCount, todayMinutes, streak, weekCount, weekMinutes);
    }
}