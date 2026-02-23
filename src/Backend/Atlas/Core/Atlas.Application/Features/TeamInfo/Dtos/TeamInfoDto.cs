namespace Atlas.Application.Features.TeamInfo.Dtos;

public record TeamInfoDto(
    Guid TeamId,
    string TeamName,
    Guid OwnerUserId,
    TeamObjectiveDto? ActiveObjective,
    List<TeamRosterMemberDto> Roster,
    TeamArmoryDto? Armory,
    List<TeamVaultLinkDto> VaultLinks
);

public record TeamObjectiveDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime? Deadline,
    bool IsActive
);

public record TeamRosterMemberDto(
    Guid MemberId,
    Guid UserId,
    string Role,
    string? CurrentFocus,
    DateTime JoinedAt
);

public record TeamArmoryDto(
    Guid Id,
    string StagingServerUrl,
    bool IsStagingOnline,
    string? TestAccountEmail,
    string? TestAccountPassword,
    string? ProductionVersion,
    string? StagingVersion
);

public record TeamVaultLinkDto(
    Guid Id,
    string Label,
    string Url,
    string? Icon,
    int SortOrder
);

