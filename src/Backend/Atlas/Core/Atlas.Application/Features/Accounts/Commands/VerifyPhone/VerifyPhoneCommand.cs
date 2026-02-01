using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.VerifyPhone;

public record VerifyPhoneCommand(
    string PhoneNumber,
    string VerificationCode
) : IRequest<bool>;

