using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetColorTrend;

public class GetColorTrendQueryHandler(
    IDesignInsightCalculationService designInsight,
    ICurrentUserService currentUser
) : IRequestHandler<GetColorTrendQuery, Dictionary<string, int>>
{
    public async Task<Dictionary<string, int>> Handle(GetColorTrendQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await designInsight.GetColorTrendAsync(userId, cancellationToken);
    }
}

