using Atlas.Application.Features.Teams.Dtos;
using MediatR;

namespace Atlas.Application.Features.Teams.Queries.GetMyTeams;

public record GetMyTeamsQuery : IRequest<List<TeamDto>>;

