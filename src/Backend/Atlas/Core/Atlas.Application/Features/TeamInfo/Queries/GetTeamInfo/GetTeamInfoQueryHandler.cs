using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.TeamInfo.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.TeamInfo.Queries.GetTeamInfo;

public class GetTeamInfoQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetTeamInfoQuery, TeamInfoDto>
{
    public async Task<TeamInfoDto> Handle(GetTeamInfoQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        if (team.Members.All(m => m.UserId != userId))
            throw new ForbiddenException("You are not a member of this team.");

        var objective = await dbContext.TeamObjectives
            .Where(o => o.TeamId == request.TeamId && o.IsActive && !o.IsDeleted)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        var memberFocuses = await dbContext.TeamMemberFocuses
            .Where(f => f.TeamId == request.TeamId && f.IsActive && !f.IsDeleted)
            .ToListAsync(cancellationToken);

        var armory = await dbContext.TeamArmories
            .FirstOrDefaultAsync(a => a.TeamId == request.TeamId && !a.IsDeleted, cancellationToken);

        var vaultLinks = await dbContext.TeamVaultLinks
            .Where(v => v.TeamId == request.TeamId && !v.IsDeleted)
            .OrderBy(v => v.SortOrder)
            .ToListAsync(cancellationToken);

        var roster = team.Members.Where(m => !m.IsDeleted).Select(m =>
        {
            var focus = memberFocuses.FirstOrDefault(f => f.TeamMemberId == m.Id);
            return new TeamRosterMemberDto(
                m.Id,
                m.UserId,
                m.Role.ToString(),
                focus?.FocusDescription,
                m.JoinedAt);
        }).ToList();

        return new TeamInfoDto(
            team.Id,
            team.Name,
            team.OwnerUserId,
            objective != null
                ? new TeamObjectiveDto(objective.Id, objective.Title, objective.Description, objective.Deadline, objective.IsActive)
                : null,
            roster,
            armory != null
                ? new TeamArmoryDto(armory.Id, armory.StagingServerUrl, armory.IsStagingOnline,
                    armory.TestAccountEmail, armory.TestAccountPassword,
                    armory.ProductionVersion, armory.StagingVersion)
                : null,
            vaultLinks.Select(v => new TeamVaultLinkDto(v.Id, v.Label, v.Url, v.Icon, v.SortOrder)).ToList()
        );
    }
}

