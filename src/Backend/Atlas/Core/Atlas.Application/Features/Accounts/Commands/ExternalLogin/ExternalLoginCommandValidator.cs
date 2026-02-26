using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.")
            .Must(provider => string.Equals(provider, "github", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Unsupported provider. Supported provider: GitHub.");

        RuleFor(x => x)
            .Must(cmd => !string.IsNullOrEmpty(cmd.AccessToken) || !string.IsNullOrEmpty(cmd.AuthorizationCode))
            .When(x => string.Equals(x.Provider, "github", StringComparison.OrdinalIgnoreCase))
            .WithMessage("For GitHub login either AccessToken or AuthorizationCode must be provided.");
    }
}
