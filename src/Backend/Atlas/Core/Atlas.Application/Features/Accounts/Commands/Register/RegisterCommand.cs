using Atlas.Application.Common.Models;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public record RegisterCommand(
    string FullName,
    string UserName,
    string Email,
    string? PhoneNumber,
    string Password,
    string ConfirmPassword,
    UserVerificationChannel? PhoneVerificationChannel
) : IRequest<ResponseModel<bool>>;