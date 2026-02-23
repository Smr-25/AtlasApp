using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.CreateDecisionLog;

public class CreateDecisionLogCommandHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<CreateDecisionLogCommand, DecisionLogEntry>
{
    public Task<DecisionLogEntry> Handle(CreateDecisionLogCommand request, CancellationToken cancellationToken)
    {
        var result = utilityService.CreateDecisionLogEntry(request.Decision, request.Rationale, request.DecidedBy);
        return Task.FromResult(result);
    }
}

