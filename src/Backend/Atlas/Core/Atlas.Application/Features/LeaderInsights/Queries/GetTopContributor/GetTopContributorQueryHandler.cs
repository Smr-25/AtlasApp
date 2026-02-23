using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetTopContributor;

public class GetTopContributorQueryHandler(
    ILeaderInsightCalculationService insightService
) : IRequestHandler<GetTopContributorQuery, TopContributorResult>
{
    public async Task<TopContributorResult> Handle(GetTopContributorQuery request, CancellationToken cancellationToken)
    {
        return await insightService.GetTopContributorAsync(request.TeamId, request.From, request.To, cancellationToken);
    }
}

