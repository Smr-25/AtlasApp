using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetZeroIncidentStreak;

public record GetZeroIncidentStreakQuery : IRequest<ZeroIncidentStreakResult>;

public record ZeroIncidentStreakResult(int Days, DateTime? LastIncidentDate);

