using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.CheckSeoMeta;

public class CheckSeoMetaQueryHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<CheckSeoMetaQuery, SeoMetaCheckResult>
{
    public Task<SeoMetaCheckResult> Handle(CheckSeoMetaQuery request, CancellationToken cancellationToken)
    {
        var result = marketerUtility.CheckSeoMeta(request.Title, request.Description, request.Url);
        return Task.FromResult(result);
    }
}

