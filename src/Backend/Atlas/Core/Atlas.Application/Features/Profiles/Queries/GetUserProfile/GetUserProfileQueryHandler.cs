using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Profiles.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Profiles.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) : IRequestHandler<GetUserProfileQuery, UserProfileDetailDto>
{
    public async Task<UserProfileDetailDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();
        
        var profile = await applicationDbContext.UserProfiles
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        if (profile == null)
            throw new NotFoundException("User Profile", userId);

        var answers = await applicationDbContext.OnboardingAnswers
            .Where(a => a.UserId == userId && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var optionIds = answers.Select(a => a.OptionId).Distinct().ToList();
        var options = await applicationDbContext.OnboardingOptions
            .Where(o => optionIds.Contains(o.Id) && !o.IsDeleted)
            .ToDictionaryAsync(o => o.Id, o => o.Text, cancellationToken);

        var answerDtos = answers.Select(a => new Atlas.Application.Features.Onboarding.Dtos.OnboardingAnswerDto(
            a.QuestionId,
            a.OptionId,
            options.ContainsKey(a.OptionId) ? options[a.OptionId] : string.Empty,
            a.CustomValue
        )).ToList();

        var tags = options.Values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return new UserProfileDetailDto(
            profile.Id,
            profile.JobTitle,
            profile.Bio ?? string.Empty,
            profile.ThemeColor,
            profile.Profession.ToString(),
            profile.Workspaces,
            tags,
            answerDtos
        );
    }
}