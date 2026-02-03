namespace Atlas.Application.Features.Snippets.Dtos;

public record SnippetDto(
    Guid Id,
    string Title,
    string Code,
    string Language,
    string[] Tags,    
    bool IsFavorite,
    DateTimeOffset CreatedAt
);