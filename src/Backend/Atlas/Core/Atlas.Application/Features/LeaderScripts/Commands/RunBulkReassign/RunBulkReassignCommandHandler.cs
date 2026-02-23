using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunBulkReassign;

public class RunBulkReassignCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunBulkReassignCommand, BulkReassignResult>
{
    public async Task<BulkReassignResult> Handle(RunBulkReassignCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.BulkReassignTasksAsync(userId, request.AbsentMemberId, request.TeamId, cancellationToken);
    }
}

