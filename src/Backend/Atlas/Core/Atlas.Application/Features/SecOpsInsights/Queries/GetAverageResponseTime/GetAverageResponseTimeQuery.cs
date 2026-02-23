using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetAverageResponseTime;

public record GetAverageResponseTimeQuery(DateTime From, DateTime To) : IRequest<AverageResponseTimeResult>;

public record AverageResponseTimeResult(double AverageMinutes, double FastestMinutes, double SlowestMinutes);

