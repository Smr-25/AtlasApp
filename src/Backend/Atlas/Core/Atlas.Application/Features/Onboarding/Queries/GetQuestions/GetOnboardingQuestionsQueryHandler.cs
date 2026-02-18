using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Onboarding.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Queries.GetQuestions;

public class GetOnboardingQuestionsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper)
    : IRequestHandler<GetOnboardingQuestionsQuery, List<OnboardingQuestionDto>>
{
    public async Task<List<OnboardingQuestionDto>> Handle(GetOnboardingQuestionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = applicationDbContext.OnboardingQuestions.AsQueryable();

        query = request.Profession.HasValue ? query.Where(q => q.TargetProfession == null || q.TargetProfession == request.Profession) : query.Where(q => q.TargetProfession == null);
        return await query
            .OrderBy(q => q.Order)
            .ProjectTo<OnboardingQuestionDto>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }
}