using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Commands.CreateQuestion;

public class CreateOnboardingQuestionCommandHandler(IApplicationDbContext applicationDbContext) 
    : IRequestHandler<CreateOnboardingQuestionCommand, Guid>
{
    public async Task<Guid> Handle(CreateOnboardingQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = OnboardingQuestion.Create(
            request.Text, 
            request.Order, 
            request.IsMultiSelect, 
            request.TargetProfession
        );
        await applicationDbContext.OnboardingQuestions.AddAsync(question, cancellationToken);
        await applicationDbContext.SaveChangesAsync(cancellationToken);
        return question.Id;
    }
}