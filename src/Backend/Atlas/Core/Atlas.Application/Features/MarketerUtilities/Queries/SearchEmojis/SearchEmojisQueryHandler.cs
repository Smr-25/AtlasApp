using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.SearchEmojis;

public class SearchEmojisQueryHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<SearchEmojisQuery, List<EmojiResult>>
{
    public Task<List<EmojiResult>> Handle(SearchEmojisQuery request, CancellationToken cancellationToken)
    {
        var result = marketerUtility.SearchEmojis(request.Query);
        return Task.FromResult(result);
    }
}

