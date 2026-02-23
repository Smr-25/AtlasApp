namespace Atlas.Application.Common.Interfaces;

public interface ISecOpsInsightCalculationService
{
    Task<int> GetThreatsBlockedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> GetVulnerabilitiesPatchedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<double> GetAverageResponseTimeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<double> GetSecurityScoreAsync(Guid userId, CancellationToken ct);
    Task<int> GetZeroIncidentStreakAsync(Guid userId, CancellationToken ct);
    Task<long> GetScannedBytesAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<Dictionary<DateTime, int>> GetOpenPortsGraphAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
}

