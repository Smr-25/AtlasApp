using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetAbTestWinRate;

public record GetAbTestWinRateQuery(DateTime From, DateTime To) : IRequest<AbTestWinRateResult>;

public record AbTestWinRateResult(double WinRate, int TotalTests, int Wins);

