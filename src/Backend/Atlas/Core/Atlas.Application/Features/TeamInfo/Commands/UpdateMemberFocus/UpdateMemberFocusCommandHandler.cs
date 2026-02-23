using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.TeamInfo.Commands.UpdateMemberFocus;

public class UpdateMemberFocusCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateMemberFocusCommand, Guid>
{
    public async Task<Guid> Handle(UpdateMemberFocusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var member = await dbContext.TeamMembers
            .FirstOrDefaultAsync(m => m.TeamId == request.TeamId && m.UserId == userId && !m.IsDeleted, cancellationToken)
            ?? throw new ForbiddenException("You are not a member of this team.");

        var existingFocus = await dbContext.TeamMemberFocuses
            .FirstOrDefaultAsync(f => f.TeamMemberId == member.Id && f.IsActive && !f.IsDeleted, cancellationToken);

        if (existingFocus != null)
        {
            existingFocus.Deactivate();
        }

        var focus = TeamMemberFocus.Create(request.TeamId, member.Id, request.FocusDescription);
        await dbContext.TeamMemberFocuses.AddAsync(focus, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return focus.Id;
    }
}

