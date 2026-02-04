namespace Atlas.Application.Features.System.Dtos;

public record AiHealthAdviceDto(
    string Summary,          
    string ActionableAdvice, 
    bool IsCritical,         
    string OptimizedMode     
);