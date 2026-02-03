using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Dashboard.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;
        var userName = currentUserService.UserName;

        var totalWorkspaces = await applicationDbContext.Workspaces
            .CountAsync(w => w.Persona.UserId.Equals(userId) && !w.IsDeleted, cancellationToken);

        var totalIntegrations = await applicationDbContext.Integrations
            .CountAsync(i => i.Persona.UserId.Equals(userId) && !i.IsDeleted, cancellationToken);

        var activeLinks = await applicationDbContext.WorkspaceIntegrations
            .CountAsync(wi => wi.Workspace.Persona.UserId.Equals(userId) && !wi.IsDeleted, cancellationToken);

        var lastWorkspaces = await applicationDbContext.Workspaces
            .Where(w => w.Persona.UserId.Equals(userId) && !w.IsDeleted)
            .OrderByDescending(w => w.ModifiedAt ?? w.CreatedAt)
            .Take(5)
            .Select(w => new { w.Id, w.Name, Date = w.ModifiedAt ?? w.CreatedAt, Type = "Workspace" })
            .ToListAsync(cancellationToken);

        var lastIntegrations = await applicationDbContext.Integrations
            .Where(i => i.Persona.UserId.Equals(userId) && !i.IsDeleted)
            .OrderByDescending(i => i.ModifiedAt ?? i.CreatedAt)
            .Take(5)
            .Select(i => new { i.Id, i.Name, Date = i.ModifiedAt ?? i.CreatedAt, Type = "Integration" })
            .ToListAsync(cancellationToken);

        var activities = lastWorkspaces.Concat(lastIntegrations)
            .OrderByDescending(x => x.Date)
            .Take(5)
            .Select(x => new RecentActivityDto(
                x.Id,
                x.Name,
                x.Type,
                "Updated", 
                GetTimeAgo(x.Date)
            ))
            .ToList();

        var hour = DateTime.UtcNow.AddHours(4).Hour; 
        var greeting = hour switch
        {
            < 12 => $"Sabahın xeyir ☀️, {userName}",
            < 18 => $"Hər vaxtın xeyir 👋 , {userName}",
            < 24 => "Axşamınız xeyir 🌆 , {userName}",
            _ => "Gecəiniz xeyir 🌙 , {userName}"
        };

        return new DashboardStatsDto(
            greeting,
            totalWorkspaces,
            totalIntegrations,
            activeLinks,
            activities
        );
    }
    
    private static string GetTimeAgo(DateTimeOffset date)
    {
        var span = DateTimeOffset.UtcNow - date;
        return span.TotalMinutes switch
        {
            < 1 => "İndi",
            < 60 => $"{span.Minutes} dəq əvvəl",
            _ => span.TotalHours < 24 ? $"{span.Hours} saat əvvəl" : $"{span.Days} gün əvvəl"
        };
    }
}