using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetCostPerFeature;

public record GetCostPerFeatureQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<CostPerFeatureResult>;

