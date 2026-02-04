namespace Atlas.Application.Features.Communication.Dtos;

public record EmailDto(
    string Id,
    string From,       
    string Subject,    
    string Snippet,    
    string Date       
);