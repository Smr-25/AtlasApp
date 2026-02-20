namespace Atlas.Application.Features.Focus.Dtos;

public record FocusStatsDto(
    int TotalSessionsToday,      
    int TotalMinutesToday,       
    int CurrentStreak,
    int TotalSessionsThisWeek,
    int TotalMinutesThisWeek
);