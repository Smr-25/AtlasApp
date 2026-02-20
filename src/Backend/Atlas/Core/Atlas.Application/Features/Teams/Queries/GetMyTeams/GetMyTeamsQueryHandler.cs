using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Teams.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Teams.Queries.GetMyTeams;

public class GetMyTeamsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetMyTeamsQuery, List<TeamDto>>
{
    public async Task<List<TeamDto>> Handle(GetMyTeamsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var teams = await dbContext.Teams
            .Include(t => t.Members)
            .Where(t => t.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);

        return teams.Select(t => new TeamDto(
            t.Id,
            t.Name,
            t.OwnerUserId,
            t.MaxMembers,
            t.Members.Count,
            t.Members.Select(m => new TeamMemberDto(
                m.Id,
                m.UserId,
                m.Role.ToString(),
                m.JoinedAt
            )).ToList()
        )).ToList();
    }
}

