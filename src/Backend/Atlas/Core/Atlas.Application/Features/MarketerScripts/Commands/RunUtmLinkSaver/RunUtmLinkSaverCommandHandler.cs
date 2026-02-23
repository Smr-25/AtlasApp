using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunUtmLinkSaver;

public class RunUtmLinkSaverCommandHandler : IRequestHandler<RunUtmLinkSaverCommand, string>
{
    public Task<string> Handle(RunUtmLinkSaverCommand request, CancellationToken cancellationToken)
    {
        var separator = request.BaseUrl.Contains('?') ? "&" : "?";
        var utmUrl = $"{request.BaseUrl}{separator}utm_source={Uri.EscapeDataString(request.Source)}&utm_medium={Uri.EscapeDataString(request.Medium)}&utm_campaign={Uri.EscapeDataString(request.Campaign)}";
        return Task.FromResult(utmUrl);
    }
}

