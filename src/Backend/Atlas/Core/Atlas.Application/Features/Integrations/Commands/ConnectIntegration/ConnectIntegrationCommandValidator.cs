using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.ConnectIntegration;

public class ConnectIntegrationCommandValidator : AbstractValidator<ConnectIntegrationCommand>
{
    public ConnectIntegrationCommandValidator()
    {
        RuleFor(x => x.AccessToken).NotEmpty()
            .WithMessage("AccessToken is required."); 
        RuleFor(x => x.Name).NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters.");
        RuleFor(x => x.Provider).IsInEnum()
            .WithMessage("Provider is required.");
    }
}