using MediatR;

namespace Atlas.Application.Features.SecOpsInsights.Queries.GetThreatsBlocked;

public record GetThreatsBlockedQuery(DateTime From, DateTime To) : IRequest<ThreatsBlockedResult>;

public record ThreatsBlockedResult(int TotalBlocked, int DdosBlocked, int MalwareBlocked, int BruteForceBlocked);

