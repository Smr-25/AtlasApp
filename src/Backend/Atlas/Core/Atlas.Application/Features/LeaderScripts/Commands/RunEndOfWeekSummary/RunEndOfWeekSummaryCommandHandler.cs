using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunEndOfWeekSummary;

public class RunEndOfWeekSummaryCommandHandler(
    ILeaderScriptService scriptService,
    ICurrentUserService currentUser
) : IRequestHandler<RunEndOfWeekSummaryCommand, WeekSummaryResult>
{
    public async Task<WeekSummaryResult> Handle(RunEndOfWeekSummaryCommand request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await scriptService.GenerateWeekSummaryAsync(userId, request.TeamId, cancellationToken);
    }
}

