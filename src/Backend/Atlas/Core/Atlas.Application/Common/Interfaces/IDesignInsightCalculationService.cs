namespace Atlas.Application.Common.Interfaces;

public interface IDesignInsightCalculationService
{
    Task<double> GetAssetsOptimizedSavingsAsync(Guid userId, CancellationToken ct);
    Task<int> GetHandoffsCompletedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<double> GetFigmaFocusTimeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<Dictionary<string, int>> GetColorTrendAsync(Guid userId, CancellationToken ct);
    Task<double> GetReusabilityScoreAsync(Guid userId, CancellationToken ct);
    Task<double> GetBrowserFreeHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> GetDesignDebtCountAsync(Guid userId, CancellationToken ct);
}

