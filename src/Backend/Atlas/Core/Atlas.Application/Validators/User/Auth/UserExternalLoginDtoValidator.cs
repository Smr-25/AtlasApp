using Atlas.Application.Dtos.Users.Auth;
using FluentValidation;

namespace Atlas.Application.Validators.User.Auth;

public class UserExternalLoginDtoValidator : AbstractValidator<UserExternalLoginDto>
{
    public UserExternalLoginDtoValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(provider => provider == "Apple" || provider == "Google" || provider == "Facebook")
            .WithMessage("Unsupported provider. Supported providers are Apple, Google, and Facebook.");

        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("IdToken is required.");
        
        When(x => x.Provider == "Apple", () =>
        {
            RuleFor(x => x.AuthorizationCode)
                .NotEmpty().WithMessage("AuthorizationCode is required for Apple login.");
        });
        When(x => x.Provider == "Google", () =>
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("AccessToken is required for Google login.");
        });
        When(x => x.Provider == "Facebook", () =>
        {
            RuleFor(x => x.AccessToken)
                .NotEmpty().WithMessage("AccessToken is required for Facebook login.");
        });
    }
}