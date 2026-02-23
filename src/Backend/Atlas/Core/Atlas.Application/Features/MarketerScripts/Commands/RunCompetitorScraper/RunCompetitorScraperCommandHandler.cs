using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunCompetitorScraper;

public class RunCompetitorScraperCommandHandler(
    IScriptRunnerService scriptRunner
) : IRequestHandler<RunCompetitorScraperCommand, string>
{
    public async Task<string> Handle(RunCompetitorScraperCommand request, CancellationToken cancellationToken)
    {
        var result = await scriptRunner.ExecuteAsync(
            "curl", $"-s -L -o /dev/null -w '%{{http_code}}' {request.CompetitorUrl}", ".", cancellationToken);
        return $"Competitor URL: {request.CompetitorUrl}\nStatus: {result}";
    }
}

