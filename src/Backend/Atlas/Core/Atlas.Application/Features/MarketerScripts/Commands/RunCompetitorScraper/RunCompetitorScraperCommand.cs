using MediatR;

namespace Atlas.Application.Features.MarketerScripts.Commands.RunCompetitorScraper;

public record RunCompetitorScraperCommand(string CompetitorUrl) : IRequest<string>;

