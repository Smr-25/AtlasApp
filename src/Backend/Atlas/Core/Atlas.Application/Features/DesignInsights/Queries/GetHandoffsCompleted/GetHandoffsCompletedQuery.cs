using MediatR;

namespace Atlas.Application.Features.DesignInsights.Queries.GetHandoffsCompleted;

public record GetHandoffsCompletedQuery(DateTime From, DateTime To) : IRequest<int>;

