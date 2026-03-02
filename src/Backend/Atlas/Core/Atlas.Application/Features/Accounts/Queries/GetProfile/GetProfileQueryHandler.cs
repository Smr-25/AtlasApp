using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Exceptions.Users;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Queries.GetProfile;

public class GetProfileQueryHandler(
    UserManager<AppUser> userManager, 
    ICurrentUserService currentUserService,
    IApplicationDbContext applicationDbContext) : IRequestHandler<GetProfileQuery, AccountDto>
{
    public async Task<AccountDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authenticated");

        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null)
            throw new NotFoundException(nameof(AppUser));

        var profile = await applicationDbContext.UserProfiles.FirstOrDefaultAsync(p => p.Id == user.Id, cancellationToken);
        string? bio = null;
        List<string>? tags = null;

        if (profile != null)
        {
            bio = profile.Bio;

            var answers = await applicationDbContext.OnboardingAnswers
                .Where(a => a.UserId == profile.Id && !a.IsDeleted)
                .ToListAsync(cancellationToken);

            if (answers.Count > 0)
            {
                var optionIds = answers.Select(a => a.OptionId).Distinct().ToList();
                var options = await applicationDbContext.OnboardingOptions
                    .Where(o => optionIds.Contains(o.Id) && !o.IsDeleted)
                    .ToDictionaryAsync(o => o.Id, o => o.Text, cancellationToken);

                tags = options.Values
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .Select(v => v.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        var dto = new AccountDto(
            user.Id.ToString(),
            user.UserName,
            user.Email ?? string.Empty,
            user.FullName,
            user.PhoneNumber,
            user.EmailConfirmed,
            user.PhoneNumber != null,
            user.CreatedAt,
            user.Status,
            user.LastLoginAt,
            bio,
            tags
        );

        return dto;
    }
}