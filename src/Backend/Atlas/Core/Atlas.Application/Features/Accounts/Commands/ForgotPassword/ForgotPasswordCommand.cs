using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ForgotPassword;

public record ForgotPasswordCommand
(
    string Email
): IRequest<bool>;
