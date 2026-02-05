using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Onboarding.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Queries.GetQuestions;

public class GetOnboardingQuestionsQueryHandler(IApplicationDbContext applicationDbContext) 
    : IRequestHandler<GetOnboardingQuestionsQuery, List<OnboardingQuestionDto>>
{
    public async Task<List<OnboardingQuestionDto>> Handle(GetOnboardingQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await applicationDbContext.OnboardingQuestions
            .Include(q => q.Options)
            .Where(q => q.TargetProfession == null || q.TargetProfession == request.Profession)
            .OrderBy(q => q.Order)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return questions.Select(q => new OnboardingQuestionDto(
            q.Id,
            q.Text,
            q.IsMultiSelect,
            q.Options.Select(o => new OnboardingOptionDto(o.Id, o.Text)).ToList()
        )).ToList();
    }
}