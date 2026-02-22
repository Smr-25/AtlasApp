using MediatR;

namespace Atlas.Application.Features.ProactiveAgents.Queries.SummarizePr;

public record SummarizePrQuery(string PrDiff, string PrTitle) : IRequest<string>;

