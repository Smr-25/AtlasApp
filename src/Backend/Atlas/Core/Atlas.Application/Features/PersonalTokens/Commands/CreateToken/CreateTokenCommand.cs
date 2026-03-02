using System.Security.Cryptography;
using System.Text;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.PersonalTokens.Dtos;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.PersonalTokens.Commands.CreateToken;

public record CreateTokenCommand(
    string Name,
    string[] Scopes,
    DateTime? ExpiresAt = null
) : IRequest<CreatedTokenDto>;

public class CreateTokenCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<CreateTokenCommand, CreatedTokenDto>
{
    public async Task<CreatedTokenDto> Handle(CreateTokenCommand request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        var rawToken = $"atlas_{Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)).Replace("+", "").Replace("/", "").Replace("=", "")}";
        var tokenPrefix = rawToken[..12];
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken))).ToLower();

        var token = PersonalAccessToken.Create(
            userId, request.Name, tokenHash, tokenPrefix,
            request.Scopes, request.ExpiresAt);

        await context.PersonalAccessTokens.AddAsync(token, ct);
        await context.SaveChangesAsync(ct);

        return new CreatedTokenDto(token.Id, token.Name, rawToken, token.Scopes, token.ExpiresAt);
    }
}
