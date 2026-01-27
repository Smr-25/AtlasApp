using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.AddPhoneNumber;

public record AddPhoneNumberCommand(
    Guid UserId,
    string PhoneNumber,
    UserVerificationChannel VerificationChannel
) : IRequest<ResponseModel<bool>>;