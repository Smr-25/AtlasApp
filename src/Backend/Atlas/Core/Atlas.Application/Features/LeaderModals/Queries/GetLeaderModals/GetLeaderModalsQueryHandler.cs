using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.LeaderModals.Queries.GetLeaderModals;

public class GetLeaderModalsQueryHandler(
    IApplicationDbContext dbContext,
    ICurrentUserService currentUser
) : IRequestHandler<GetLeaderModalsQuery, List<LeaderModalDto>>
{
    public async Task<List<LeaderModalDto>> Handle(GetLeaderModalsQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await dbContext.LeaderModalStates
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new LeaderModalDto(m.Id, m.ModalType.ToString(), m.HasBeenSeen, m.DismissedAt, m.PayloadJson, m.TeamId))
            .ToListAsync(cancellationToken);
    }
}

