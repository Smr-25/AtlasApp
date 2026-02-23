using MediatR;

namespace Atlas.Application.Features.MarketerUtilities.Queries.ConvertMarkdownToHtml;

public record ConvertMarkdownToHtmlQuery(string Markdown) : IRequest<string>;

