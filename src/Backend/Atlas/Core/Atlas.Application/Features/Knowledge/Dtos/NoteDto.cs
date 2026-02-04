namespace Atlas.Application.Features.Knowledge.Dtos;

public record NoteDto(
    string Id,
    string Title,
    string Url,        
    string Icon,       
    DateTime LastEdited
);