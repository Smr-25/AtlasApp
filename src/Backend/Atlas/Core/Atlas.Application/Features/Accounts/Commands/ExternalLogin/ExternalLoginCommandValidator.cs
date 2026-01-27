using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.ExternalLogin;

public class ExternalLoginCommandValidator : AbstractValidator<ExternalLoginCommand>
{
    private static readonly string[] SupportedProviders = { "google", "apple" };

    public ExternalLoginCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.")
            .Must(provider => SupportedProviders.Contains(provider.ToLower()))
            .WithMessage("Unsupported provider. Supported providers: Google, Apple.");

        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage("IdToken is required.")
            .MinimumLength(50)
            .WithMessage("IdToken appears to be invalid.");
    }
}
