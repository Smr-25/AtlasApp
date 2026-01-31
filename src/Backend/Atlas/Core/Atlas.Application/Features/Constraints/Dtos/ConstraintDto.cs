using Atlas.Domain.Enums;

namespace Atlas.Application.Features.Constraints.Dtos;

public record ConstraintDto(
    Guid Id,
    ConstraintType Type,
    string Description,
    int ImpactLevel,
    bool IsActive,
    DateTime CreatedAt
);