using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderInsights.Queries.GetTeamMood;

public record GetTeamMoodQuery(Guid TeamId, DateTime From, DateTime To) : IRequest<TeamMoodResult>;

