using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ChangePassword;

public record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
) : IRequest<bool>;
