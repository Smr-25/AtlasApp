using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetDesignDebt;

public record GetDesignDebtQuery() : IRequest<int>;

