namespace Atlas.Application.Features.Accounts.Dtos;

public record VerifyResetCodeResponseDto(
    string ResetToken,
    DateTime ExpiresAt
);

