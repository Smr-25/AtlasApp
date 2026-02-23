using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.EstimateCost;

public class EstimateCostQueryHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<EstimateCostQuery, CostEstimateResult>
{
    public Task<CostEstimateResult> Handle(EstimateCostQuery request, CancellationToken cancellationToken)
    {
        var result = utilityService.EstimateCost(request.HoursEstimated, request.HourlyRate, request.ServerMonthlyCost, request.EstimatedMonths);
        return Task.FromResult(result);
    }
}

