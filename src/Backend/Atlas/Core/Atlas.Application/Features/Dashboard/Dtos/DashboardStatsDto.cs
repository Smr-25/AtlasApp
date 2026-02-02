namespace Atlas.Application.Features.Dashboard.Dtos;

public record DashboardStatsDto(
    string Greeting,            
    int TotalWorkspaces,        
    int TotalIntegrations,      
    int ActiveLinks,            
    List<RecentActivityDto> RecentActivities
);