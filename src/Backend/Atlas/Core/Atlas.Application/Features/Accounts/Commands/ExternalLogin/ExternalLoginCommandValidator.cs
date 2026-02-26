using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    public ExternalLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.")
            .Must(provider => string.Equals(provider, "github", StringComparison.OrdinalIgnoreCase) || string.Equals(provider, "google", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Unsupported provider. Supported providers: Google, GitHub.");

        RuleFor(x => x)
            .Must(cmd => !string.IsNullOrEmpty(cmd.AccessToken) || !string.IsNullOrEmpty(cmd.AuthorizationCode))
            .When(x => string.Equals(x.Provider, "github", StringComparison.OrdinalIgnoreCase))
            .WithMessage("For GitHub login either AccessToken or AuthorizationCode must be provided.");

        RuleFor(x => x.IdToken)
            .NotEmpty()
            .When(x => string.Equals(x.Provider, "google", StringComparison.OrdinalIgnoreCase))
            .WithMessage("For Google login IdToken must be provided.");
    }
}
