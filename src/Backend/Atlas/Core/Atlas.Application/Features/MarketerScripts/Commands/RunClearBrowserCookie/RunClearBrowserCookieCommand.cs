using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunClearBrowserCookie;

public record RunClearBrowserCookieCommand(string Browser = "chrome") : IRequest<string>;

