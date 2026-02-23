using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetBlockedTime;

public record GetBlockedTimeQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<BlockedTimeResult>;

