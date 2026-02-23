using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetZeroIncidentStreak;

public class GetZeroIncidentStreakQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetZeroIncidentStreakQuery, ZeroIncidentStreakResult>
{
    public async Task<ZeroIncidentStreakResult> Handle(GetZeroIncidentStreakQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var days = await insightService.GetZeroIncidentStreakAsync(userId, cancellationToken);
        return new ZeroIncidentStreakResult(days, null);
    }
}

