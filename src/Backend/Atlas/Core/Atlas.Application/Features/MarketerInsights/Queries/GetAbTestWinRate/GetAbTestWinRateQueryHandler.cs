using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetAbTestWinRate;

public class GetAbTestWinRateQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetAbTestWinRateQuery, AbTestWinRateResult>
{
    public async Task<AbTestWinRateResult> Handle(GetAbTestWinRateQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var winRate = await insightService.GetAbTestWinRateAsync(userId, request.From, request.To, cancellationToken);
        return new AbTestWinRateResult(winRate, 0, 0);
    }
}

