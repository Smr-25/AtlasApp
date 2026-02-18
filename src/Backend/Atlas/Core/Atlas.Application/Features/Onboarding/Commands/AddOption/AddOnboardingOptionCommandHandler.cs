using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Commands.AddOption;

public class AddOnboardingOptionCommandHandler(IApplicationDbContext applicationDbContext) 
    : IRequestHandler<AddOnboardingOptionCommand, Guid>
{
    public async Task<Guid> Handle(AddOnboardingOptionCommand request, CancellationToken cancellationToken)
    {
        var question = await applicationDbContext.OnboardingQuestions
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question == null) throw new NotFoundException("Question", request.QuestionId);

        var option = OnboardingOption.Create(
            request.Text, 
            request.QuestionId,
            request.RecommendedIntegration, 
            request.RecommendedTemplate
        );
        
        question.AddOption(option); 
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return option.Id;
    }
}