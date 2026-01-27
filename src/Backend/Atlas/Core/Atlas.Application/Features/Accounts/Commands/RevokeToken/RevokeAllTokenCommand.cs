using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.RevokeToken;

public record RevokeAllTokenCommand() : IRequest<Unit>;