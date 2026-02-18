namespace Atlas.Application.Features.Accounts.Dtos;

public record RegisterResponseDto(
    bool Success,
    bool RequiresEmailVerification,
    bool RequiresPhoneVerification,
    string? TelegramBotLink
);

