using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.SquadArena.Commands.AwardBadge;

public class AwardBadgeCommandHandler(
    IApplicationDbContext dbContext
) : IRequestHandler<AwardBadgeCommand, Guid>
{
    public async Task<Guid> Handle(AwardBadgeCommand request, CancellationToken cancellationToken)
    {
        var entry = SquadArenaEntry.Create(request.TeamId, request.UserId, request.BadgeType, request.Points, request.SprintId);
        dbContext.SquadArenaEntries.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entry.Id;
    }
}

