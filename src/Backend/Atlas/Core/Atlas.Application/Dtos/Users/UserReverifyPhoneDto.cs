using Atlas.Domain.Enums;

namespace Atlas.Application.Dtos.Users;

public record UserReverifyPhoneDto(
    string PhoneNumber,
    UserVerificationChannel UserVerificationChannel
);