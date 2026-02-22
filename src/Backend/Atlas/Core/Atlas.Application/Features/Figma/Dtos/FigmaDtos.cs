namespace Atlas.Application.Features.Figma.Dtos;

public record FigmaCommentDto(
    string Id,
    string Message,
    string AuthorName,
    string AuthorAvatarUrl,
    DateTime CreatedAt,
    bool IsResolved,
    string? ParentId);

public record FigmaComponentDto(
    string Key,
    string Name,
    string Description,
    string ThumbnailUrl,
    string ContainingFrameName);

