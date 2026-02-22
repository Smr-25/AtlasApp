using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetColorTrend;

public record GetColorTrendQuery() : IRequest<Dictionary<string, int>>;

