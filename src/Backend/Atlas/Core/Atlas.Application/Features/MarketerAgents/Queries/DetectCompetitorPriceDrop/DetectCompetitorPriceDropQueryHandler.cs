using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectCompetitorPriceDrop;

public class DetectCompetitorPriceDropQueryHandler(
    IMarketerAgentService agentService
) : IRequestHandler<DetectCompetitorPriceDropQuery, List<CompetitorPriceResult>>
{
    public async Task<List<CompetitorPriceResult>> Handle(DetectCompetitorPriceDropQuery request, CancellationToken cancellationToken)
    {
        return await agentService.DetectCompetitorPriceDropAsync(request.CompetitorUrl, cancellationToken);
    }
}

