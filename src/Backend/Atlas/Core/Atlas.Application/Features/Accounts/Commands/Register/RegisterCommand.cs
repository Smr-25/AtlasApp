using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.Register;

public record RegisterCommand(
    string FullName,
    string UserName,
    string Email,
    string Password
) : IRequest<AuthResponseDto>;
