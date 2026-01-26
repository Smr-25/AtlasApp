using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.RevokeToken;

public record RevokeTokenCommand(string RefreshToken) : IRequest<Unit>;