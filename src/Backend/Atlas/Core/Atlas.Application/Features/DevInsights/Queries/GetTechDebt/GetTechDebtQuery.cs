using MediatR;

namespace Atlas.Application.Features.DevInsights.Queries.GetTechDebt;

public record GetTechDebtQuery(string ProjectPath) : IRequest<TechDebtResult>;

public record TechDebtResult(int TodoCount, int FixmeCount, int HackCount, int TotalDebt);

