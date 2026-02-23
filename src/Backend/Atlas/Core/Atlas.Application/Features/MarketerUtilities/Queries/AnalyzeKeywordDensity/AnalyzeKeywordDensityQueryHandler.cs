using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.AnalyzeKeywordDensity;

public class AnalyzeKeywordDensityQueryHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<AnalyzeKeywordDensityQuery, KeywordDensityResult>
{
    public Task<KeywordDensityResult> Handle(AnalyzeKeywordDensityQuery request, CancellationToken cancellationToken)
    {
        var result = marketerUtility.AnalyzeKeywordDensity(request.Content, request.Keyword);
        return Task.FromResult(result);
    }
}

