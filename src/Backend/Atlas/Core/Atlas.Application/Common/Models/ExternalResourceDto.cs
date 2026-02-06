namespace Atlas.Application.Common.Models;

public record ExternalResourceDto(
    string Id,          
    string Name,        
    string? Description,
    string? Url,        
    string Type,
    Dictionary<string, string> Metadata
);