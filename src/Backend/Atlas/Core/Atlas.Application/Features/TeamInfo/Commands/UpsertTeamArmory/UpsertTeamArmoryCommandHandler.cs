using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.TeamInfo.Commands.UpsertTeamArmory;

public class UpsertTeamArmoryCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpsertTeamArmoryCommand, Guid>
{
    public async Task<Guid> Handle(UpsertTeamArmoryCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var team = await dbContext.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == request.TeamId, cancellationToken)
            ?? throw new NotFoundException("Team", request.TeamId);

        var member = team.Members.FirstOrDefault(m => m.UserId == userId && !m.IsDeleted);
        if (member == null || member.Role != TeamMemberRole.Manager)
            throw new ForbiddenException("Only team managers can manage armory.");

        var armory = await dbContext.TeamArmories
            .FirstOrDefaultAsync(a => a.TeamId == request.TeamId && !a.IsDeleted, cancellationToken);

        if (armory != null)
        {
            if (request.TestAccountEmail != null && request.TestAccountPassword != null)
                armory.UpdateTestAccount(request.TestAccountEmail, request.TestAccountPassword);
            armory.UpdateVersions(request.ProductionVersion, request.StagingVersion);
            await dbContext.SaveChangesAsync(cancellationToken);
            return armory.Id;
        }

        armory = TeamArmory.Create(
            request.TeamId,
            request.StagingServerUrl,
            request.TestAccountEmail,
            request.TestAccountPassword,
            request.ProductionVersion);

        await dbContext.TeamArmories.AddAsync(armory, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return armory.Id;
    }
}

