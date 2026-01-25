using Atlas.Application.Dtos.Users.State;
using FluentValidation;

namespace Atlas.Application.Validators.User.State;

public class PersonaStateUpdateDtoValidator : AbstractValidator<PersonaStateUpdateDto>
{
    public PersonaStateUpdateDtoValidator()
    {
        RuleFor(x => x.CurrentPhase)
            .NotEmpty().WithMessage("CurrentPhase is required.")
            .MaximumLength(100).WithMessage("CurrentPhase must not exceed 100 characters.");

        RuleFor(x => x.MentalLoadLevel)
            .NotEmpty().WithMessage("MentalLoadLevel is required.")
            .MaximumLength(100).WithMessage("MentalLoadLevel must not exceed 100 characters.");

    }
}