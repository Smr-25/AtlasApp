using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Onboarding.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Onboarding.Queries.GetProfessionQuestion;

public class GetProfessionQuestionQueryHandler(IApplicationDbContext context, IMapper mapper)
    : IRequestHandler<GetProfessionQuestionQuery, OnboardingQuestionDto?>
{
    public async Task<OnboardingQuestionDto?> Handle(GetProfessionQuestionQuery request, CancellationToken cancellationToken)
    {
        return await context.OnboardingQuestions
            .Where(q => q.Order == 1 && q.TargetProfession == null)
            .Include(q => q.Options)
            .ProjectTo<OnboardingQuestionDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}

