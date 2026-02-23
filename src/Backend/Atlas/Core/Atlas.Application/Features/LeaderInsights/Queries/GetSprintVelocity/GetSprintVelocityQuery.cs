using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetSprintVelocity;

public record GetSprintVelocityQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<SprintVelocityResult>;

