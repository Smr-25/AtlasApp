using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.UpdateIntegration;

public class UpdateIntegrationCommandValidator : AbstractValidator<UpdateIntegrationCommand>
{
    public UpdateIntegrationCommandValidator()
    {
        RuleFor(x => x.IntegrationId).NotEmpty()
            .WithMessage("IntegrationId is required.");
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters.");
    }
}

