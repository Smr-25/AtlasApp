using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.ReconnectIntegration;

public class ReconnectIntegrationCommandValidator : AbstractValidator<ReconnectIntegrationCommand>
{
    public ReconnectIntegrationCommandValidator()
    {
        RuleFor(x => x.IntegrationId)
            .NotEmpty()
            .WithMessage("IntegrationId is required.");

        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage("AccessToken is required.");
    }
}

