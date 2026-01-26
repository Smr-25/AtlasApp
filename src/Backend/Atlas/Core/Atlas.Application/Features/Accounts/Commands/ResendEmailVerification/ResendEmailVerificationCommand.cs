using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ResendEmailVerification;

public record ResendEmailVerificationCommand(
    string Email
) : IRequest<ResponseModel<bool>>;