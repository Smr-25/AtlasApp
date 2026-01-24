using Atlas.Domain.Enums;

namespace Atlas.Application.Dtos.Users.Profile;

public record UserAddPhoneNumberDto
(
    string Email,
    string PhoneNumber,
    UserVerificationChannel UserVerificationChannel
);