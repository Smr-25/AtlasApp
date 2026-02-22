using Atlas.Application.Features.Perplexity.Dtos;
using MediatR;

namespace Atlas.Application.Features.Perplexity.Queries.SearchError;

public record SearchErrorQuery(string ErrorMessage, string? StackTrace) : IRequest<PerplexitySearchResultDto>;

