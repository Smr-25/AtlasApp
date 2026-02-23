using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunStandupPing;

public class RunStandupPingCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunStandupPingCommand, string>
{
    public async Task<string> Handle(RunStandupPingCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.SendStandupPingAsync(userId, request.TeamId, cancellationToken);
    }
}

