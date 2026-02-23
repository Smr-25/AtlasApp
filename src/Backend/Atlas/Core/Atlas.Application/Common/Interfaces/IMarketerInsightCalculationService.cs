namespace Atlas.Application.Common.Interfaces;

public interface IMarketerInsightCalculationService
{
    Task<double> GetTotalRoasAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> GetLeadsGeneratedAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<int> GetZombieAdsKilledAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<double> GetAbTestWinRateAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<Dictionary<int, double>> GetPeakEngagementHoursAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<SentimentResult> GetAudienceSentimentAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
    Task<double> GetTimeSavedOnReportingAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);
}

public record SentimentResult(double PositivePercent, double NegativePercent, double NeutralPercent, int TotalMentions);

