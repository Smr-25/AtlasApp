using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetThreatsBlocked;

public class GetThreatsBlockedQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetThreatsBlockedQuery, ThreatsBlockedResult>
{
    public async Task<ThreatsBlockedResult> Handle(GetThreatsBlockedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var total = await insightService.GetThreatsBlockedAsync(userId, request.From, request.To, cancellationToken);
        return new ThreatsBlockedResult(total, 0, 0, 0);
    }
}

