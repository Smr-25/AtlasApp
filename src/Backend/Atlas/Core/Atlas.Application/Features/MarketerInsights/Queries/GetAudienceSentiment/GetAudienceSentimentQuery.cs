using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerInsights.Queries.GetAudienceSentiment;

public record GetAudienceSentimentQuery(DateTime From, DateTime To) : IRequest<SentimentResult>;

