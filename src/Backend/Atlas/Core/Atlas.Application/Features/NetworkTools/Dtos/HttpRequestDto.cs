namespace Atlas.Application.Features.NetworkTools.Dtos;

public record HttpRequestDto(
    string Url,            
    string Method,          
    string? Body,           
    Dictionary<string, string>? Headers 
);