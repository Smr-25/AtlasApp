using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Teams.Dtos;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Queries.GetTeamRadar;

public class GetTeamRadarQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    UserManager<AppUser> userManager)
    : IRequestHandler<GetTeamRadarQuery, TeamRadarDto>
{
    public async Task<TeamRadarDto> Handle(GetTeamRadarQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (user.Role != UserRole.TeamLeader)
            throw new ForbiddenException("Only Team Leaders can access the team radar.");

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.OwnerUserId != userId)
            throw new ForbiddenException("You can only view radar for teams you own.");

        var today = DateTime.UtcNow.Date;
        var memberUserIds = team.Members.Select(m => m.UserId).ToList();

        // Get today's focus sessions for all team members
        var focusSessions = await dbContext.FocusSessions
            .Where(fs => memberUserIds.Contains(fs.UserId) && fs.StartedAt >= today)
            .ToListAsync(cancellationToken);

        // Get latest activity for each member
        var latestActivities = await dbContext.UserActivities
            .Where(ua => memberUserIds.Contains(ua.UserId))
            .GroupBy(ua => ua.UserId)
            .Select(g => g.OrderByDescending(ua => ua.CreatedAt).First())
            .ToListAsync(cancellationToken);

        var memberDtos = new List<TeamRadarMemberDto>();

        foreach (var member in team.Members)
        {
            var memberUser = await userManager.FindByIdAsync(member.UserId.ToString());
            var memberFocusSessions = focusSessions.Where(fs => fs.UserId == member.UserId).ToList();
            var latestActivity = latestActivities.FirstOrDefault(a => a.UserId == member.UserId);

            // Check if member has an active focus session
            var activeSession = memberFocusSessions
                .FirstOrDefault(fs => fs.Status == FocusSessionStatus.InProgress);

            string? activeWorkspaceName = null;
            if (activeSession?.WorkspaceId != null)
            {
                var workspace = await dbContext.Workspaces
                    .FirstOrDefaultAsync(w => w.Id == activeSession.WorkspaceId, cancellationToken);
                activeWorkspaceName = workspace?.Name;
            }

            memberDtos.Add(new TeamRadarMemberDto(
                member.UserId,
                memberUser?.UserName ?? "Unknown",
                member.Role.ToString(),
                memberFocusSessions.Where(fs => fs.Status == FocusSessionStatus.Completed)
                    .Sum(fs => fs.DurationMinutes),
                memberFocusSessions.Count(fs => fs.Status == FocusSessionStatus.Completed),
                activeWorkspaceName,
                latestActivity?.ActionType,
                latestActivity?.CreatedAt.DateTime
            ));
        }

        return new TeamRadarDto(
            team.Id,
            team.Name,
            memberDtos.Count(m => m.ActiveWorkspaceName != null),
            memberDtos.Sum(m => m.TodayFocusMinutes),
            memberDtos
        );
    }
}

