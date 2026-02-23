using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SquadRadar.Commands.UpdatePresence;

public class UpdatePresenceCommandHandler(
    ISquadRadarService radarService,
    ICurrentUserService currentUser
) : IRequestHandler<UpdatePresenceCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePresenceCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        await radarService.UpdatePresenceAsync(userId, request.TeamId, request.Status, request.ToolIcon, request.Focus, request.MeetingMinutesLeft, cancellationToken);
        return Unit.Value;
    }
}

