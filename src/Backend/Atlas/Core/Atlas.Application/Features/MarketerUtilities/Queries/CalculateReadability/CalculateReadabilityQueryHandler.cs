using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.CalculateReadability;

public class CalculateReadabilityQueryHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<CalculateReadabilityQuery, ReadabilityResult>
{
    public Task<ReadabilityResult> Handle(CalculateReadabilityQuery request, CancellationToken cancellationToken)
    {
        var result = marketerUtility.CalculateReadability(request.Text);
        return Task.FromResult(result);
    }
}

