using MediatR;

namespace Atlas.Application.Features.TeamInfo.Commands.UpsertTeamArmory;

public record UpsertTeamArmoryCommand(
    Guid TeamId,
    string StagingServerUrl,
    string? TestAccountEmail,
    string? TestAccountPassword,
    string? ProductionVersion,
    string? StagingVersion
) : IRequest<Guid>;

