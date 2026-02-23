using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetPeakEngagementHours;

public record GetPeakEngagementHoursQuery(DateTime From, DateTime To) : IRequest<PeakEngagementHoursResult>;

public record PeakEngagementHoursResult(Dictionary<int, double> HourlyEngagement);

