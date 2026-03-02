using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.PersonalTokens.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonalTokens.Queries.GetTokens;

public record GetTokensQuery : IRequest<List<PersonalTokenDto>>;

public class GetTokensQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService) : IRequestHandler<GetTokensQuery, List<PersonalTokenDto>>
{
    public async Task<List<PersonalTokenDto>> Handle(GetTokensQuery request, CancellationToken ct)
    {
        var userId = currentUserService.GetRequiredUserId();

        return await context.PersonalAccessTokens
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new PersonalTokenDto(
                t.Id, t.Name, t.TokenPrefix, t.Scopes,
                t.ExpiresAt, t.LastUsedAt, t.IsRevoked, t.CreatedAt))
            .ToListAsync(ct);
    }
}

