using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.GetViralTrends;

public record GetViralTrendsQuery(string Industry) : IRequest<List<TrendResult>>;

