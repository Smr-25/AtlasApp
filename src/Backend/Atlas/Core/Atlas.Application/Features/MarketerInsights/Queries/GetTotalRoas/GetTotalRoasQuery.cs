using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetTotalRoas;

public record GetTotalRoasQuery(DateTime From, DateTime To) : IRequest<TotalRoasResult>;

public record TotalRoasResult(double Roas, double TotalSpend, double TotalRevenue);

