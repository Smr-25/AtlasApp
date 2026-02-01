using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.VerifyEmail;

public record VerifyEmailCommand(
string Email,
string VerificationCode
) : IRequest<bool>;
