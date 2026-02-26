using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    UserManager<AppUser> userManager) 
    : IRequestHandler<CompleteOnboardingCommand, Guid>
{
    public async Task<Guid> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        if (await applicationDbContext.UserProfiles.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new Exception("Profile already exists.");

        var role = request.Profession switch
        {
            UserProfession.Developer => UserRole.Developer,
            UserProfession.Designer => UserRole.Designer,
            UserProfession.CyberSecurity => UserRole.SecOps,
            UserProfession.DigitalMarketing => UserRole.Marketer,
            UserProfession.ProductManager => UserRole.TeamLeader,
            _ => UserRole.Developer
        };

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new Exception("User not found.");

        user.AssignRole(role);
        await userManager.UpdateAsync(user);

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains(role.ToString()))
        {
            await userManager.AddToRoleAsync(user, role.ToString());
        }

        var userProfile = AppUserProfile.Create(userId, request.Profession, request.JobTitle);

        var defaultWorkspace = Workspace.Create("Main Workspace", userId, isDefault: true);
        await applicationDbContext.Workspaces.AddAsync(defaultWorkspace, cancellationToken); 


        await applicationDbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        if (request.Answers is { Count: > 0 })
        {
            var optionIds = request.Answers.Select(a => a.OptionId).ToList();
            var options = await applicationDbContext.OnboardingOptions
                .Where(o => optionIds.Contains(o.Id) && !o.IsDeleted)
                .ToListAsync(cancellationToken);

            var answersToSave = request.Answers.Select(a => new OnboardingAnswer
            {
                UserId = userId,
                QuestionId = a.QuestionId,
                OptionId = a.OptionId,
                CustomValue = a.CustomValue
            }).ToList();

            await applicationDbContext.OnboardingAnswers.AddRangeAsync(answersToSave, cancellationToken);

            var recommendedGroups = options
                .Where(o => !string.IsNullOrWhiteSpace(o.RecommendedIntegration))
                .Select(o => o.RecommendedIntegration!.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var rec in recommendedGroups)
            {
                if (!Enum.TryParse<IntegrationProvider>((ReadOnlySpan<char>)rec, ignoreCase: true, out var provider))
                {
                    continue;
                }

                var existingIntegration = await applicationDbContext.Integrations
                    .FirstOrDefaultAsync(i => i.UserProfileId == userId && i.Provider == provider && !i.IsDeleted, cancellationToken);

                Integration integration;
                if (existingIntegration != null)
                {
                    integration = existingIntegration;
                }
                else
                {
                    integration = Integration.CreatePlaceholder(userId, provider, rec);
                    await applicationDbContext.Integrations.AddAsync(integration, cancellationToken);
                }

                var linkExists = await applicationDbContext.WorkspaceIntegrations
                    .AnyAsync(wi => wi.WorkspaceId == defaultWorkspace.Id && wi.IntegrationId == integration.Id && !wi.IsDeleted, cancellationToken);

                if (!linkExists)
                {
                    var link = new WorkspaceIntegration
                    {
                        WorkspaceId = defaultWorkspace.Id,
                        IntegrationId = integration.Id,
                        Enabled = false
                    };
                    await applicationDbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
                }
            }

            var selectedOptionTexts = options.Select(o => o.Text).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var bioParts = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.JobTitle))
                bioParts.Add(request.JobTitle.Trim());

            if (selectedOptionTexts.Count > 0)
                bioParts.AddRange(selectedOptionTexts.Take(10)); // limit number of tags in bio

            var bio = string.Join(" — ", bioParts).Trim();
            if (bio.Length > 1000) bio = bio[..1000];

            userProfile.UpdateInfo(request.JobTitle, string.IsNullOrWhiteSpace(bio) ? null : bio);

            await applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        return userId;
    }
}