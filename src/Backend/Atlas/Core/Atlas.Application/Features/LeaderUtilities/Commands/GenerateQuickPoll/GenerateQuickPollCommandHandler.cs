using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.GenerateQuickPoll;

public class GenerateQuickPollCommandHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<GenerateQuickPollCommand, QuickPollResult>
{
    public Task<QuickPollResult> Handle(GenerateQuickPollCommand request, CancellationToken cancellationToken)
    {
        var result = utilityService.GenerateQuickPoll(request.Question, request.Options);
        return Task.FromResult(result);
    }
}

