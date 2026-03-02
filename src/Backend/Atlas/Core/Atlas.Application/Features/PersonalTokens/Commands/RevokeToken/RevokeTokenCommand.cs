using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonalTokens.Commands.RevokeToken;

public record RevokeTokenCommand(Guid TokenId) : IRequest;

public class RevokeTokenCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<RevokeTokenCommand>
{
    public async Task Handle(RevokeTokenCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();
        var token = await context.PersonalAccessTokens
            .FirstOrDefaultAsync(t => t.Id == request.TokenId && t.UserId == userId, ct)
            ?? throw new NotFoundException("Token", request.TokenId);

        token.Revoke();
        await context.SaveChangesAsync(ct);
    }
}

