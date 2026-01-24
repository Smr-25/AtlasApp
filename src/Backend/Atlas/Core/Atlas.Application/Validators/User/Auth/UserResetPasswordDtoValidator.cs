using Atlas.Application.Dtos.Users.Auth;
using FluentValidation;

namespace Atlas.Application.Validators.User.Auth;

public class UserResetPasswordDtoValidator : AbstractValidator<UserResetPasswordDto>
{
    public UserResetPasswordDtoValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Email) || !string.IsNullOrEmpty(x.UserName))
            .WithMessage("Either Email or UserName is required.");
        
        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email format.");
        
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.");
        
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
    }
}