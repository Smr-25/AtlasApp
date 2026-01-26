using FluentValidation;

namespace Atlas.Application.Features.Accounts.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.FullName)
            .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.UserName)
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Username cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Username can only contain letters, numbers, dots, underscores, and hyphens.")
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}