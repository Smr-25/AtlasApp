using Atlas.Application.Features.Accounts.Dtos;
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
    UserRole Role,
    UserVerificationChannel? PhoneVerificationChannel
) : IRequest<RegisterResponseDto>;
