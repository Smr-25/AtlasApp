using Atlas.Application.Features.Onboarding.Dtos;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Queries.GetProfessionQuestion;

public record GetProfessionQuestionQuery : IRequest<OnboardingQuestionDto?>;

