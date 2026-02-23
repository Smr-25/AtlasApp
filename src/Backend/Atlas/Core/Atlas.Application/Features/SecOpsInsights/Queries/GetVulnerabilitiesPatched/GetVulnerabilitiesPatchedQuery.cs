using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetVulnerabilitiesPatched;

public record GetVulnerabilitiesPatchedQuery(DateTime From, DateTime To) : IRequest<VulnerabilitiesPatchedResult>;

public record VulnerabilitiesPatchedResult(int TotalPatched, int Critical, int High, int Medium, int Low);

