using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetFocusHeatmap;

public record GetFocusHeatmapQuery(DateTime From, DateTime To) : IRequest<Dictionary<string, double>>;

