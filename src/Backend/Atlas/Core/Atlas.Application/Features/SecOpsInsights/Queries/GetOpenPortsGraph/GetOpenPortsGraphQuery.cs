using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetOpenPortsGraph;

public record GetOpenPortsGraphQuery(DateTime From, DateTime To) : IRequest<OpenPortsGraphResult>;

public record OpenPortsGraphResult(Dictionary<DateTime, int> DataPoints);

