using Atlas.Domain.Enums;

namespace Atlas.Application.Dtos.Users.Profile;

public record UserAddPhoneNumberDto
(
    string PhoneNumber,
    UserVerificationChannel UserVerificationChannel
);