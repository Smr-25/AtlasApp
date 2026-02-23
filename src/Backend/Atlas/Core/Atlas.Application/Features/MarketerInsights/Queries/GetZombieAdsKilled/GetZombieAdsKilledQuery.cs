using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetZombieAdsKilled;

public record GetZombieAdsKilledQuery(DateTime From, DateTime To) : IRequest<ZombieAdsKilledResult>;

public record ZombieAdsKilledResult(int TotalKilled, double MoneySaved);

