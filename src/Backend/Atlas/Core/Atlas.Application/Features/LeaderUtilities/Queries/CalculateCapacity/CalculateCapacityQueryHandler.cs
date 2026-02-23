using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.CalculateCapacity;

public class CalculateCapacityQueryHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<CalculateCapacityQuery, CapacityResult>
{
    public Task<CapacityResult> Handle(CalculateCapacityQuery request, CancellationToken cancellationToken)
    {
        var result = utilityService.CalculateCapacity(request.Members);
        return Task.FromResult(result);
    }
}

