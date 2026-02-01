using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string VerificationCode,
    string NewPassword,
    string ConfirmPassword
) : IRequest<bool>;
