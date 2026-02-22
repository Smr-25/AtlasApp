using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignUtilities.Queries.CheckContrast;

public class CheckContrastQueryHandler(
    IDesignUtilityService designUtility
) : IRequestHandler<CheckContrastQuery, ContrastCheckResult>
{
    public Task<ContrastCheckResult> Handle(CheckContrastQuery request, CancellationToken cancellationToken)
    {
        var result = designUtility.CheckContrast(request.ForegroundHex, request.BackgroundHex);
        return Task.FromResult(result);
    }
}

