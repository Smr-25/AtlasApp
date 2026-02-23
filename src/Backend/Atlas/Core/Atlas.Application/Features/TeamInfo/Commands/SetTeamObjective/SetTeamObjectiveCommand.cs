using MediatR;

namespace Atlas.Application.Features.TeamInfo.Commands.SetTeamObjective;

public record SetTeamObjectiveCommand(
    Guid TeamId,
    string Title,
    string? Description,
    DateTime? Deadline
) : IRequest<Guid>;

