using Atlas.Application.Features.Onboarding.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand(
    Guid UserId,
    UserProfession Profession,
    string JobTitle,
    List<AnswerDto> Answers
) : IRequest<Guid>;