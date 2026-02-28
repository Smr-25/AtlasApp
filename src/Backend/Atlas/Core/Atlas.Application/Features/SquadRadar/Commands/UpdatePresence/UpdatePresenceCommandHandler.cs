using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SquadRadar.Commands.UpdatePresence;

public class UpdatePresenceCommandHandler(
    ISquadRadarService radarService,
    ICurrentUserService currentUser,
    IAtlasHubService hubService
) : IRequestHandler<UpdatePresenceCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePresenceCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        await radarService.UpdatePresenceAsync(userId, request.TeamId, request.Status, request.ToolIcon, request.Focus, request.MeetingMinutesLeft, cancellationToken);

        var currentFocus = request.Focus;
        var lastActiveAt = DateTime.UtcNow;
        var payload = new
        {
            userId,
            status = request.Status.ToString(),
            toolIcon = request.ToolIcon,
            currentFocus,
            meetingMinutesLeft = request.MeetingMinutesLeft,
            lastActiveAt
        };

        await hubService.SendPresenceUpdateAsync(request.TeamId, payload, cancellationToken);

        return Unit.Value;
    }
}
