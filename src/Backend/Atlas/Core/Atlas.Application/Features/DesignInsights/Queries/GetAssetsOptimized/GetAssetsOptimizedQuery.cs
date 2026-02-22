using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetAssetsOptimized;

public record GetAssetsOptimizedQuery() : IRequest<AssetsOptimizedResult>;

public record AssetsOptimizedResult(double TotalSavedMb, int TotalOptimized);

