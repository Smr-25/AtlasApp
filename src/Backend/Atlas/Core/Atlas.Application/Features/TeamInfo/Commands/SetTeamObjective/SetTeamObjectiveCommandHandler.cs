using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.TeamInfo.Commands.SetTeamObjective;

public class SetTeamObjectiveCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<SetTeamObjectiveCommand, Guid>
{
    public async Task<Guid> Handle(SetTeamObjectiveCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        var member = team.Members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null || member.Role != TeamMemberRole.Manager)
            throw new ForbiddenException("Only team managers can set objectives.");

        var currentActive = await dbContext.TeamObjectives
            .Where(o => o.TeamId == request.TeamId && o.IsActive && !o.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var obj in currentActive)
            obj.Complete();

        var objective = TeamObjective.Create(request.TeamId, request.Title, request.Description, request.Deadline);
        await dbContext.TeamObjectives.AddAsync(objective, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return objective.Id;
    }
}

