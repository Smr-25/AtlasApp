using MediatR;

namespace Atlas.Application.Features.Onboarding.Commands.AddOption;

public record AddOnboardingOptionCommand(
    Guid QuestionId,
    string Text,
    string? RecommendedIntegration, 
    string? RecommendedTemplate    
) : IRequest<Guid>;