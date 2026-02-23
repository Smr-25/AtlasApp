using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunUtmLinkSaver;

public record RunUtmLinkSaverCommand(string BaseUrl, string Source, string Medium, string Campaign) : IRequest<string>;

