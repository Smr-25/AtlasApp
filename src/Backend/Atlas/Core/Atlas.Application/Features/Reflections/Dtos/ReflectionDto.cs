using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Reflections.Dtos;

public record ReflectionDto(
    Guid Id,
    ReflectionType Type,
    string Content,
    int? MoodScore,
    List<string> Tags,
    Guid? DecisionId,
    DateTime CreatedAt,
    bool IsPrivate
);