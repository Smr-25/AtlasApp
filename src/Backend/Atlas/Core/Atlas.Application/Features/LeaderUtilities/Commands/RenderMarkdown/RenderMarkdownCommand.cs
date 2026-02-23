using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.RenderMarkdown;

public record RenderMarkdownCommand(string Markdown) : IRequest<string>;

