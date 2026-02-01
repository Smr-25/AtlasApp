using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.UpdateProfile;

public record UpdateProfileCommand(
    string? FullName,
    string? UserName
) : IRequest<AccountDto>;
