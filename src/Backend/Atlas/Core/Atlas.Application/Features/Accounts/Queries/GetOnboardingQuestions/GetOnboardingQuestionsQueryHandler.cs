using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Accounts.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Accounts.Queries.GetOnboardingQuestions;

public class GetOnboardingQuestionsQueryHandler(IApplicationDbContext applicationDbContext) : IRequestHandler<GetOnboardingQuestionsQuery, List<OnboardingQuestionDto>>
{
    public async Task<List<OnboardingQuestionDto>> Handle(GetOnboardingQuestionsQuery request, CancellationToken cancellationToken)
    {
        return await applicationDbContext.OnboardingQuestions
            .Where(q => q.ProfessionId == request.ProfessionId)
            .Include(q => q.Options)
            .OrderBy(q => q.Order)
            .Select(q => new OnboardingQuestionDto(
                q.Id,
                q.Text,
                q.IsMultiSelect,
                q.Options.Select(o => new OnboardingOptionDto(o.Id, o.Text)).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}