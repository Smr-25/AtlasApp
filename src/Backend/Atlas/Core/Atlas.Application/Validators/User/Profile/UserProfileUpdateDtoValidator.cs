using Atlas.Application.Dtos.Users.Profile;
using FluentValidation;

namespace Atlas.Application.Validators.User.Profile;

public class UserProfileUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
{
    public UserProfileUpdateDtoValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.UserName)
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens.")
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}