using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunSprintStarter;

public class RunSprintStarterCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunSprintStarterCommand, SprintStarterResult>
{
    public async Task<SprintStarterResult> Handle(RunSprintStarterCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.RunSprintStarterAsync(userId, request.SprintName, request.InitialTasks, request.TeamId, cancellationToken);
    }
}

