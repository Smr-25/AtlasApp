using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.TestRegex;

public record TestRegexQuery(string Pattern, string Input) : IRequest<TestRegexResult>;

public record TestRegexResult(bool IsMatch, int MatchCount, List<RegexMatchResult> Matches);

