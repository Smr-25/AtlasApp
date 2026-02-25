using Atlas.Application.Features.Onboarding.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand(
    UserProfession Profession,
    string JobTitle,
    List<AnswerDto> Answers
) : IRequest<Guid>;