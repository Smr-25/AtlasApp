using MediatR;

namespace Atlas.Application.Features.Teams.Commands.CreateTeam;

public record CreateTeamCommand(string Name) : IRequest<Guid>;

