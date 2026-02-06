using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService) 
    : IRequestHandler<CompleteOnboardingCommand, Guid>
{
    public async Task<Guid> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        if (await applicationDbContext.UserProfiles.AnyAsync(u => u.Id == userId, cancellationToken))
            throw new Exception("Profile already exists.");

        var userProfile = AppUserProfile.Create(userId, request.Profession, request.JobTitle);

        var defaultWorkspace = Workspace.Create("Main Workspace", userId, isDefault: true);
        await applicationDbContext.Workspaces.AddAsync(defaultWorkspace, cancellationToken); 

        var selectedOptionIds = request.Answers.Select(a => a.OptionId).ToList();
        var selectedOptions = await applicationDbContext.OnboardingOptions
            .Where(o => selectedOptionIds.Contains(o.Id))
            .ToListAsync(cancellationToken);

        foreach (var option in selectedOptions)
        {
            if (!string.IsNullOrEmpty(option.RecommendedIntegration) && 
                Enum.TryParse<IntegrationProvider>(option.RecommendedIntegration, true, out var provider))
            {
                var existingIntegration = userProfile.Integrations.FirstOrDefault(i => i.Provider == provider);
                
                if (existingIntegration == null)
                {
                    var placeholder = Integration.CreatePlaceholder(
                        userId, 
                        provider, 
                        $"{provider}" 
                    );
                    
                    userProfile.AddIntegration(placeholder);
                    
                    var link = new WorkspaceIntegration
                    {
                        Workspace = defaultWorkspace,
                        Integration = placeholder
                    };
                    await applicationDbContext.WorkspaceIntegrations.AddAsync(link, cancellationToken);
                }
            }
        }

        
        await applicationDbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return userId;
    }
}