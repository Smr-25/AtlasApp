using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.DeleteIntegration;

public class DeleteIntegrationCommandValidator : AbstractValidator<DeleteIntegrationCommand>
{
    public DeleteIntegrationCommandValidator()
    {
        RuleFor(x => x.IntegrationId).NotEmpty()
            .WithMessage("IntegrationId is required.");
    }
}

