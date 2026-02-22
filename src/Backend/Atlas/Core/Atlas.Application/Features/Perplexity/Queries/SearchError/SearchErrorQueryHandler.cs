using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Perplexity.Dtos;
using MediatR;

namespace Atlas.Application.Features.Perplexity.Queries.SearchError;

public class SearchErrorQueryHandler(
    IPerplexityAdapter perplexityAdapter
) : IRequestHandler<SearchErrorQuery, PerplexitySearchResultDto>
{
    public async Task<PerplexitySearchResultDto> Handle(SearchErrorQuery request, CancellationToken cancellationToken)
    {
        var answer = await perplexityAdapter.SearchWithContextAsync(
            request.ErrorMessage, request.StackTrace ?? "", cancellationToken);

        return new PerplexitySearchResultDto(answer, [], request.ErrorMessage);
    }
}

