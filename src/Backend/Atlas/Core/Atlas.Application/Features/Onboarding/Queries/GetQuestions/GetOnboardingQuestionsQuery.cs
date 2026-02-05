using Atlas.Application.Features.Onboarding.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Queries.GetQuestions;

public record GetOnboardingQuestionsQuery(UserProfession Profession) : IRequest<List<OnboardingQuestionDto>>;