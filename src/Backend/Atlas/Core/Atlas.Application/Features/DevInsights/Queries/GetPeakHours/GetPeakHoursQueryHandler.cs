using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetPeakHours;

public class GetPeakHoursQueryHandler(
    IInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetPeakHoursQuery, Dictionary<int, double>>
{
    public async Task<Dictionary<int, double>> Handle(GetPeakHoursQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        return await insightService.GetPeakProductivityHoursAsync(userId, request.From, request.To, cancellationToken);
    }
}

