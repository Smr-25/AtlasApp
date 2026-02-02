namespace Atlas.Application.Features.Dashboard.Dtos;

public record RecentActivityDto(
    Guid Id,
    string Title,      
    string Type,       
    string Action,     
    string TimeAgo     
);