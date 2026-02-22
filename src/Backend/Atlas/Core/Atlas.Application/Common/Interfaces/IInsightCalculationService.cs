namespace Atlas.Application.Common.Interfaces;

public interface IInsightCalculationService
{
    Task<double> CalculateTimeSavedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<Dictionary<string, double>> GetFocusHeatmapAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> GetContextSwitchCountAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> CountTodoCommentsAsync(string projectPath, CancellationToken ct);
    Task<Dictionary<string, int>> GetCodeChurnAsync(Guid userId, Guid integrationId, CancellationToken ct);
    Task<double> GetDeploymentSuccessRateAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<Dictionary<int, double>> GetPeakProductivityHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
}

