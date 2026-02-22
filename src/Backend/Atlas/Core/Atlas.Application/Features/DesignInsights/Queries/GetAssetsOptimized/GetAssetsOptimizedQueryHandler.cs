using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetAssetsOptimized;

public class GetAssetsOptimizedQueryHandler(
    IDesignInsightCalculationService designInsight,
    ICurrentUserService currentUser
) : IRequestHandler<GetAssetsOptimizedQuery, AssetsOptimizedResult>
{
    public async Task<AssetsOptimizedResult> Handle(GetAssetsOptimizedQuery request, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(currentUser.UserId!);
        var savedMb = await designInsight.GetAssetsOptimizedSavingsAsync(userId, cancellationToken);
        return new AssetsOptimizedResult(savedMb, 0);
    }
}

