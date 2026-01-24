using Atlas.Domain.Enums;

namespace Atlas.Application.Dtos.Users.Auth;

public record UserReverifyPhoneDto(
    string PhoneNumber,
    UserVerificationChannel UserVerificationChannel
);