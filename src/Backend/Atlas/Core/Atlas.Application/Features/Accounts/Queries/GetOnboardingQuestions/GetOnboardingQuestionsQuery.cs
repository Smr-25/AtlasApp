using Atlas.Application.Features.Accounts.Dtos;
using MediatR;

namespace Atlas.Application.Features.Accounts.Queries.GetOnboardingQuestions;

public record GetOnboardingQuestionsQuery(Guid ProfessionId) : IRequest<List<OnboardingQuestionDto>>;