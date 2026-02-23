using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Queries.ConvertTimezones;

public class ConvertTimezonesQueryHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<ConvertTimezonesQuery, TimezoneConversionResult>
{
    public Task<TimezoneConversionResult> Handle(ConvertTimezonesQuery request, CancellationToken cancellationToken)
    {
        var result = utilityService.ConvertTimezones(request.Members);
        return Task.FromResult(result);
    }
}

