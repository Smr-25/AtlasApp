using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectCompetitorPriceDrop;

public record DetectCompetitorPriceDropQuery(string CompetitorUrl) : IRequest<List<CompetitorPriceResult>>;

