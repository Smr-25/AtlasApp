using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunBlockedTaskBlaster;

public class RunBlockedTaskBlasterCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunBlockedTaskBlasterCommand, BlockedTaskBlasterResult>
{
    public async Task<BlockedTaskBlasterResult> Handle(RunBlockedTaskBlasterCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.RunBlockedTaskBlasterAsync(userId, request.TeamId, cancellationToken);
    }
}

