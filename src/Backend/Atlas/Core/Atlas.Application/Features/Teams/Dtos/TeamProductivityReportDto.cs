namespace Atlas.Application.Features.Teams.Dtos;

public record TeamProductivityReportDto(
    Guid TeamId,
    string TeamName,
    DateTime WeekStart,
    DateTime WeekEnd,
    int TotalFocusMinutes,
    int TotalCompletedSessions,
    int TotalInterruptedSessions,
    double AvgSessionMinutes,
    List<MemberProductivityDto> MemberReports
);

public record MemberProductivityDto(
    Guid UserId,
    string UserName,
    int FocusMinutes,
    int CompletedSessions,
    int InterruptedSessions,
    string MostUsedTag
);

