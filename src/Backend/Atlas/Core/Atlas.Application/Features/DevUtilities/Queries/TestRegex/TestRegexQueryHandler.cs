using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.TestRegex;

public class TestRegexQueryHandler(
    IDevUtilityService devUtility
) : IRequestHandler<TestRegexQuery, TestRegexResult>
{
    public Task<TestRegexResult> Handle(TestRegexQuery request, CancellationToken cancellationToken)
    {
        var matches = devUtility.TestRegex(request.Pattern, request.Input);
        return Task.FromResult(new TestRegexResult(matches.Count > 0, matches.Count, matches));
    }
}

