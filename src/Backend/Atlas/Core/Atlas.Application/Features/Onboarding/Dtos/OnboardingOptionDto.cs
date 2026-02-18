namespace Atlas.Application.Features.Onboarding.Dtos;

public record OnboardingOptionDto(
    Guid Id, 
    string Text,
    string? RecommendedIntegration = null
);
