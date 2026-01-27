namespace Atlas.Application.Features.Accounts.Dtos;

public record AccessTokenResponseDto(
    string Token,
    DateTime Expiration
);