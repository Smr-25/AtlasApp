using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetFocusHeatmap;

public class GetFocusHeatmapQueryHandler(
    IInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetFocusHeatmapQuery, Dictionary<string, double>>
{
    public async Task<Dictionary<string, double>> Handle(GetFocusHeatmapQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetFocusHeatmapAsync(userId, request.From, request.To, cancellationToken);
    }
}

