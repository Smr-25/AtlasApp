using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.PredictBottleneck;

public record PredictBottleneckQuery(Guid TeamId) : IRequest<BottleneckResult>;

