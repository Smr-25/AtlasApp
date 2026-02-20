namespace Atlas.Application.Features.Teams.Dtos;

public record TeamDto(
    Guid Id,
    string Name,
    Guid OwnerUserId,
    int MaxMembers,
    int CurrentMemberCount,
    List<TeamMemberDto> Members
);

public record TeamMemberDto(
    Guid Id,
    Guid UserId,
    string Role,
    DateTime JoinedAt
);

