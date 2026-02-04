using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Focus.Dtos;
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

        var todaysSessions = await applicationDbContext.FocusSessions
            .Where(s => s.UserId == userId && s.CompletedAt >= today)
            .ToListAsync(cancellationToken);

        var count = todaysSessions.Count;
        var totalMinutes = todaysSessions.Sum(s => s.DurationMinutes);

        
        var streak = count > 0 ? 1 : 0; 

        return new FocusStatsDto(count, totalMinutes, streak);
    }
}