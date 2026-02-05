using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Onboarding.Commands.CreateQuestion;

public record CreateOnboardingQuestionCommand(
    string Text, 
    int Order, 
    bool IsMultiSelect, 
    UserProfession? TargetProfession 
) : IRequest<Guid>;