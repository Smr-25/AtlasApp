using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetLeadsGenerated;

public record GetLeadsGeneratedQuery(DateTime From, DateTime To) : IRequest<LeadsGeneratedResult>;

public record LeadsGeneratedResult(int TotalLeads, int OrganicLeads, int PaidLeads);

