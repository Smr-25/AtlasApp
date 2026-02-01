using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ResendPhoneVerification;

public record ResendPhoneVerificationCommand(
    string PhoneNumber,
    UserVerificationChannel Channel
) : IRequest<bool>;
