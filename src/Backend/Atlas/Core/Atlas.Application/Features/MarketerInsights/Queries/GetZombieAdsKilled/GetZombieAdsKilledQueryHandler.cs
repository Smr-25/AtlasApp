using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetZombieAdsKilled;

public class GetZombieAdsKilledQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetZombieAdsKilledQuery, ZombieAdsKilledResult>
{
    public async Task<ZombieAdsKilledResult> Handle(GetZombieAdsKilledQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var total = await insightService.GetZombieAdsKilledAsync(userId, request.From, request.To, cancellationToken);
        return new ZombieAdsKilledResult(total, 0);
    }
}

