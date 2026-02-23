using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.RenderMarkdown;

public class RenderMarkdownCommandHandler(
    ILeaderUtilityService utilityService
) : IRequestHandler<RenderMarkdownCommand, string>
{
    public Task<string> Handle(RenderMarkdownCommand request, CancellationToken cancellationToken)
    {
        var result = utilityService.RenderMarkdownPreview(request.Markdown);
        return Task.FromResult(result);
    }
}

