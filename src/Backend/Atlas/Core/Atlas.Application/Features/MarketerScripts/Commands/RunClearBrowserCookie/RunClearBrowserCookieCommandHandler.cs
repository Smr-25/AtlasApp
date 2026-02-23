using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunClearBrowserCookie;

public class RunClearBrowserCookieCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunClearBrowserCookieCommand, string>
{
    public async Task<string> Handle(RunClearBrowserCookieCommand request, CancellationToken cancellationToken)
    {
        var cookiePath = request.Browser.ToLower() switch
        {
            "chrome" => "~/Library/Application Support/Google/Chrome/Default/Cookies",
            "firefox" => "~/Library/Application Support/Firefox/Profiles/*/cookies.sqlite",
            "safari" => "~/Library/Cookies/Cookies.binarycookies",
            _ => ""
        };

        if (string.IsNullOrEmpty(cookiePath))
            return $"Unsupported browser: {request.Browser}";

        var result = await scriptRunner.ExecuteAsync(
            "bash", $"-c \"rm -f {cookiePath}\"", ".", cancellationToken);
        return $"Cookies cleared for {request.Browser}. {result}";
    }
}

