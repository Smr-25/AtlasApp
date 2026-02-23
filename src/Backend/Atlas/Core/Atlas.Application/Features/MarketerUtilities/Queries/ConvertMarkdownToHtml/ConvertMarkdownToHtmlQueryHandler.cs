using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.ConvertMarkdownToHtml;

public class ConvertMarkdownToHtmlQueryHandler(
    IMarketerUtilityService marketerUtility
) : IRequestHandler<ConvertMarkdownToHtmlQuery, string>
{
    public Task<string> Handle(ConvertMarkdownToHtmlQuery request, CancellationToken cancellationToken)
    {
        var result = marketerUtility.ConvertMarkdownToHtml(request.Markdown);
        return Task.FromResult(result);
    }
}

