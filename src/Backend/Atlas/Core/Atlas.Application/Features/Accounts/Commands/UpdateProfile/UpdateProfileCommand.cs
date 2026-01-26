using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.UpdateProfile;

public record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    string? UserName
) : IRequest<ResponseModel<AccountDto>>;