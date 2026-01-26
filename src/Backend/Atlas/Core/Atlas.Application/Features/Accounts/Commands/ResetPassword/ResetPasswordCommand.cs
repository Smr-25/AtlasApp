using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Code,
    string NewPassword,
    string ConfirmPassword
) : IRequest<ResponseModel<bool>>;