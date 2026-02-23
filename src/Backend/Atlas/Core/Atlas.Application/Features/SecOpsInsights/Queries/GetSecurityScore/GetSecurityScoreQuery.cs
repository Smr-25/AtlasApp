using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetSecurityScore;

public record GetSecurityScoreQuery : IRequest<SecurityScoreResult>;

public record SecurityScoreResult(double Score, string Grade, List<string> Recommendations);

