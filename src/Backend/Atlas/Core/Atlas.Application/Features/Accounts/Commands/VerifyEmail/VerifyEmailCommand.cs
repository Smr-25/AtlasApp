using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.VerifyEmail;

public record VerifyEmailCommand(
string Email,
string VerificationCode
) : IRequest<ResponseModel<bool>>;