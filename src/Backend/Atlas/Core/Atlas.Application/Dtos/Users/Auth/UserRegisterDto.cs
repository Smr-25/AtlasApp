using Atlas.Domain.Enums;

namespace Atlas.Application.Dtos.Users.Auth;

public record UserRegisterDto(
    string FullName,
    string UserName,
    string Email,
    string? PhoneNumber,
    string Password,
    string ConfirmPassword,
    UserVerificationChannel? PhoneVerificationChannel
);