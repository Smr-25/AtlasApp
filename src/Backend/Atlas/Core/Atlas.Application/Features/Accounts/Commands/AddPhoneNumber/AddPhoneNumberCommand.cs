using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;

public record AddPhoneNumberCommand(
    string PhoneNumber,
    UserVerificationChannel VerificationChannel
) : IRequest<bool>;
