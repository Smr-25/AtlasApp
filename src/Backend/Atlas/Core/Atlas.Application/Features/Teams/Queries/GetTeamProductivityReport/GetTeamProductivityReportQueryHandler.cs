using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Teams.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Queries.GetTeamProductivityReport;

public class GetTeamProductivityReportQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    UserManager<AppUser> userManager)
    : IRequestHandler<GetTeamProductivityReportQuery, TeamProductivityReportDto>
{
    public async Task<TeamProductivityReportDto> Handle(GetTeamProductivityReportQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (user.Role != UserRole.TeamLeader)
            throw new ForbiddenException("Only Team Leaders can access productivity reports.");

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.OwnerUserId != userId)
            throw new ForbiddenException("You can only view reports for teams you own.");

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        var memberUserIds = team.Members.Select(m => m.UserId).ToList();

        var weekSessions = await dbContext.FocusSessions
            .Where(fs => memberUserIds.Contains(fs.UserId) && fs.CreatedAt >= weekStart && fs.CreatedAt < weekEnd)
            .ToListAsync(cancellationToken);

        var memberReports = new List<MemberProductivityDto>();

        foreach (var member in team.Members)
        {
            var memberUser = await userManager.FindByIdAsync(member.UserId.ToString());
            var memberSessions = weekSessions.Where(s => s.UserId == member.UserId).ToList();
            var completed = memberSessions.Where(s => s.Status == FocusSessionStatus.Completed).ToList();
            var interrupted = memberSessions.Count(s => s.Status == FocusSessionStatus.Interrupted);
            var mostUsedTag = completed
                .GroupBy(s => s.Tag)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";

            memberReports.Add(new MemberProductivityDto(
                member.UserId,
                memberUser?.UserName ?? "Unknown",
                completed.Sum(s => s.DurationMinutes),
                completed.Count,
                interrupted,
                mostUsedTag
            ));
        }

        var allCompleted = weekSessions.Where(s => s.Status == FocusSessionStatus.Completed).ToList();

        return new TeamProductivityReportDto(
            team.Id,
            team.Name,
            weekStart,
            weekEnd,
            allCompleted.Sum(s => s.DurationMinutes),
            allCompleted.Count,
            weekSessions.Count(s => s.Status == FocusSessionStatus.Interrupted),
            allCompleted.Count > 0 ? allCompleted.Average(s => s.DurationMinutes) : 0,
            memberReports
        );
    }
}

