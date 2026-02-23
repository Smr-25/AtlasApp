using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.GetViralTrends;

public class GetViralTrendsQueryHandler(
    IMarketerAgentService agentService
) : IRequestHandler<GetViralTrendsQuery, List<TrendResult>>
{
    public async Task<List<TrendResult>> Handle(GetViralTrendsQuery request, CancellationToken cancellationToken)
    {
        return await agentService.GetViralTrendsAsync(request.Industry, cancellationToken);
    }
}

