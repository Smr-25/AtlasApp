using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.GenerateCron;

public class GenerateCronQueryHandler(
    IDevUtilityService devUtility
) : IRequestHandler<GenerateCronQuery, GenerateCronResult>
{
    public Task<GenerateCronResult> Handle(GenerateCronQuery request, CancellationToken cancellationToken)
    {
        var cron = devUtility.GenerateCron(request.Description);
        return Task.FromResult(new GenerateCronResult(cron, request.Description));
    }
}

