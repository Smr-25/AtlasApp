using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetPeakHours;

public record GetPeakHoursQuery(DateTime From, DateTime To) : IRequest<Dictionary<int, double>>;

