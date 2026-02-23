using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetPeakEngagementHours;

public class GetPeakEngagementHoursQueryHandler(
    IMarketerInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetPeakEngagementHoursQuery, PeakEngagementHoursResult>
{
    public async Task<PeakEngagementHoursResult> Handle(GetPeakEngagementHoursQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var data = await insightService.GetPeakEngagementHoursAsync(userId, request.From, request.To, cancellationToken);
        return new PeakEngagementHoursResult(data);
    }
}

