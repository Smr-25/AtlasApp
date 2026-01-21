namespace Atlas.Application.Dtos.Users;

public record UserVerifyPhoneDto
(
    string PhoneNumber,
    string Code
);