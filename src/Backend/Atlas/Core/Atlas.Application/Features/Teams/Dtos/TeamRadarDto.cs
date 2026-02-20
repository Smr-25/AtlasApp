namespace Atlas.Application.Features.Teams.Dtos;

public record TeamRadarMemberDto(
    Guid UserId,
    string UserName,
    string Role,
    int TodayFocusMinutes,
    int TodaySessions,
    string? ActiveWorkspaceName,
    string? LastActivity,
    DateTime? LastActivityTime
);

public record TeamRadarDto(
    Guid TeamId,
    string TeamName,
    int TotalMembersOnline,
    int TotalFocusMinutesToday,
    List<TeamRadarMemberDto> Members
);

