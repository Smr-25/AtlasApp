using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunSocialBlast;

public class RunSocialBlastCommandHandler(
    ISocialListeningAdapter socialAdapter
) : IRequestHandler<RunSocialBlastCommand, string>
{
    public async Task<string> Handle(RunSocialBlastCommand request, CancellationToken cancellationToken)
    {
        var results = new List<string>();
        foreach (var platform in request.Platforms)
        {
            results.Add($"Posted to {platform}: {request.Content[..Math.Min(50, request.Content.Length)]}...");
        }
        return string.Join("\n", results);
    }
}

