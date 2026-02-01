using MediatR;

namespace Atlas.Application.Features.Accounts.Commands.CompleteOnboarding;

public record CompleteOnboardingCommand(
    Guid ProfessionId,
    List<Guid> SelectedOptionIds
) : IRequest<bool>;