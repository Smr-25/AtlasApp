using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.Login;

public record LoginCommand(
    string? Email,
    string? UserName,
    string Password
) : IRequest<AuthResponseDto>;
