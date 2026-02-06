using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public class CompleteOnboardingCommandHandler(IApplicationDbContext applicationDbContext)
    : IRequestHandler<CompleteOnboardingCommand, Guid>
{
    public async Task<Guid> Handle(CompleteOnboardingCommand request, CancellationToken cancellationToken)
    {
        var userProfile = AppUserProfile.Create(request.UserId, request.Profession, request.JobTitle);

        var themeColor = request.Profession switch
        {
            UserProfession.Developer => "#007AFF", 
            UserProfession.Designer => "#FF2D55", 
            UserProfession.DevOps => "#FF9500", 
            UserProfession.DataScientist => "#FFA500", 
            UserProfession.CyberSecurity => "#34C759", 
            UserProfession.AiEngineer => "#FFA500",
            _ => "#5856D6" 
        };
        userProfile.SetTheme(themeColor); 

        var selectedOptionIds = request.Answers.Select(a => a.OptionId).ToList();

        var selectedOptions = await applicationDbContext.OnboardingOptions
            .Where(o => selectedOptionIds.Contains(o.Id))
            .ToListAsync(cancellationToken);

        var workspace = Workspace.Create("Main Workspace", userProfile.Id, isDefault: true);
        
        foreach (var record in request.Answers.Select(ans => new UserOnboardingAnswer
                 {
                     UserId = request.UserId,
                     QuestionId = ans.QuestionId,
                     OptionId = ans.OptionId
                 }))
        {
            await applicationDbContext.UserOnboardingAnswers.AddAsync(record, cancellationToken);
        }

        await applicationDbContext.UserProfiles.AddAsync(userProfile, cancellationToken);
        await applicationDbContext.Workspaces.AddAsync(workspace, cancellationToken);
        
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return userProfile.Id;
    }
}