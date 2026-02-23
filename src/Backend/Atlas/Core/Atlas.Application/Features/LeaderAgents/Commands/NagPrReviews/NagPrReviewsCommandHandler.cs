using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Commands.NagPrReviews;

public class NagPrReviewsCommandHandler(
    ILeaderAgentService agentService
) : IRequestHandler<NagPrReviewsCommand, PrReviewNagResult>
{
    public async Task<PrReviewNagResult> Handle(NagPrReviewsCommand request, CancellationToken cancellationToken)
    {
        return await agentService.NagPrReviewsAsync(request.TeamId, request.ThresholdHours, cancellationToken);
    }
}

