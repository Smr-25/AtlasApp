using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public record ExternalLoginCommand(
    string Provider,
    string IdToken,
    string? AccessToken = null,
    string? AuthorizationCode = null
): IRequest<ResponseModel<ExternalLoginResponseDto>>;