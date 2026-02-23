using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunMeetingMode;

public class RunMeetingModeCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunMeetingModeCommand, string>
{
    public async Task<string> Handle(RunMeetingModeCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.ActivateMeetingModeAsync(userId, request.DurationMinutes, cancellationToken);
    }
}

