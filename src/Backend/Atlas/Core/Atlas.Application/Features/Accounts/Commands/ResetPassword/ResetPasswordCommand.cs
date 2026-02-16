using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string ResetToken,
    string NewPassword,
    string ConfirmPassword
) : IRequest<bool>;
