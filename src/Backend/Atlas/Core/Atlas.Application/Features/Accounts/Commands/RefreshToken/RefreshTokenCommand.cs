using Atlas.Application.Common.Models;
using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ResponseModel<TokenDto>>;