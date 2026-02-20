using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Focus.Dtos;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Focus.Queries.GetActiveFocusSession;

public class GetActiveFocusSessionQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetActiveFocusSessionQuery, FocusHistoryDto?>
{
    public async Task<FocusHistoryDto?> Handle(GetActiveFocusSessionQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var session = await dbContext.FocusSessions
            .Where(fs => fs.UserId == userId && 
                         (fs.Status == FocusSessionStatus.InProgress || fs.Status == FocusSessionStatus.Paused))
            .OrderByDescending(fs => fs.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (session == null) return null;

        return new FocusHistoryDto(
            session.Id,
            session.DurationMinutes,
            session.BreakDurationMinutes,
            session.Tag,
            session.SessionType.ToString(),
            session.Status.ToString(),
            session.StartedAt,
            session.CompletedAt,
            session.WorkspaceId
        );
    }
}

