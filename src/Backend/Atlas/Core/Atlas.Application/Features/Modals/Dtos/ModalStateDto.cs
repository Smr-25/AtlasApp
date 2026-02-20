namespace Atlas.Application.Features.Modals.Dtos;

public record ModalStateDto(
    Guid Id,
    string ModalType,
    bool HasBeenSeen,
    string? PayloadJson,
    DateTimeOffset CreatedAt
);

