using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Teams.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Queries.GetTeamDashboard;

public class GetTeamDashboardQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetTeamDashboardQuery, TeamDto>
{
    public async Task<TeamDto> Handle(GetTeamDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        var isMember = team.Members.Any(m => m.UserId == userId);
        if (!isMember)
            throw new ForbiddenException("You are not a member of this team.");

        return new TeamDto(
            team.Id,
            team.Name,
            team.OwnerUserId,
            team.MaxMembers,
            team.Members.Count,
            team.Members.Select(m => new TeamMemberDto(
                m.Id,
                m.UserId,
                m.Role.ToString(),
                m.JoinedAt
            )).ToList()
        );
    }
}

