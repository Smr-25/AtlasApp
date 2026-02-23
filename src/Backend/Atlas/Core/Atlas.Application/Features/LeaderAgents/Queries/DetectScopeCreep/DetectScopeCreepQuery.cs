using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderAgents.Queries.DetectScopeCreep;

public record DetectScopeCreepQuery(Guid TeamId, string SprintId) : IRequest<ScopeCreepResult>;

