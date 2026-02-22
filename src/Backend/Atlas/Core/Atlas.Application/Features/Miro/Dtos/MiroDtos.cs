namespace Atlas.Application.Features.Miro.Dtos;

public record MiroBoardDto(
    string Id,
    string Name,
    string Description,
    string ViewLink,
    DateTime ModifiedAt,
    int StickyNoteCount);

