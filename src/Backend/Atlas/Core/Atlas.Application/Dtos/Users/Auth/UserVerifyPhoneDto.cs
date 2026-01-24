namespace Atlas.Application.Dtos.Users.Auth;

public record UserVerifyPhoneDto
(
    string PhoneNumber,
    string Code
);