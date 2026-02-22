using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.CalculateAspectRatio;

public class CalculateAspectRatioQueryHandler(
    IDesignUtilityService designUtility
) : IRequestHandler<CalculateAspectRatioQuery, AspectRatioResult>
{
    public Task<AspectRatioResult> Handle(CalculateAspectRatioQuery request, CancellationToken cancellationToken)
    {
        var result = designUtility.CalculateAspectRatio(request.Width, request.Height);
        return Task.FromResult(result);
    }
}

