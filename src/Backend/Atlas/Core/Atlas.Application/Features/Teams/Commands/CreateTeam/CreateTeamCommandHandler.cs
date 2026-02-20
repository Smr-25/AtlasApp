using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService,
    ISubscriptionGuardService subscriptionGuard,
    UserManager<AppUser> userManager)
    : IRequestHandler<CreateTeamCommand, Guid>
{
    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        // Only Team tier subscribers can create teams
        if (!await subscriptionGuard.HasTeamFeaturesAsync(userId, cancellationToken))
            throw new ForbiddenException("Team features require Team subscription.");

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("User", userId);

        if (user.Role != UserRole.TeamLeader)
            throw new ForbiddenException("Only Team Leaders can create teams.");

        var team = Team.Create(request.Name, userId);

        await dbContext.Teams.AddAsync(team, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return team.Id;
    }
}

