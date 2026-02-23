using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetSecurityScore;

public class GetSecurityScoreQueryHandler(
    ISecOpsInsightCalculationService insightService,
    ICurrentUserService currentUser
) : IRequestHandler<GetSecurityScoreQuery, SecurityScoreResult>
{
    public async Task<SecurityScoreResult> Handle(GetSecurityScoreQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var score = await insightService.GetSecurityScoreAsync(userId, cancellationToken);
        var grade = score >= 90 ? "A" : score >= 75 ? "B" : score >= 60 ? "C" : "D";
        return new SecurityScoreResult(score, grade, []);
    }
}

