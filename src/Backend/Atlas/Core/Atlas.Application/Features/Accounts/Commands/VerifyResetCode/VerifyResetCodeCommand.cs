using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.VerifyResetCode;

public record VerifyResetCodeCommand(
    string Email,
    string VerificationCode
) : IRequest<VerifyResetCodeResponseDto>;

