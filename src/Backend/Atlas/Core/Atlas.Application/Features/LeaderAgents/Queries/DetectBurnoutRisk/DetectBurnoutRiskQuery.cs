using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.DetectBurnoutRisk;

public record DetectBurnoutRiskQuery(Guid TeamId) : IRequest<BurnoutRiskResult>;

