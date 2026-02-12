namespace Atlas.Application.Features.NetworkTools.Dtos;

public record HttpResponseDto(
    int StatusCode,         
    string StatusText,      
    string Content,       
    long DurationMs,       
    bool IsSuccess
);