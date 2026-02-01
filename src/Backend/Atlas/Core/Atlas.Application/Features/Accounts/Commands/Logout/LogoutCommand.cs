using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.Logout;

public record LogoutCommand(
    string RefreshToken
) : IRequest<Unit>;