namespace Atlas.Application.Features.GitHub.Dtos;

public record GitHubActionRunDto(
    long Id,
    string Name,        
    string Status,      
    string Conclusion,
    string Branch,
    string Url,
    DateTime CreatedAt
);