using FluentValidation;

namespace Atlas.Application.Features.Profiles.Commands.UpdateProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.JobTitle).NotEmpty()
            .WithMessage("JobTitle is required.")
            .MaximumLength(100).WithMessage("JobTitle must be at most 100 characters.");
        
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio must be at most 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Bio));
        
        RuleFor(x => x.ThemeColor)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("ThemeColor must be a valid hex color (e.g., #007AFF).")
            .When(x => !string.IsNullOrEmpty(x.ThemeColor));
    }
}

