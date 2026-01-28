using FluentValidation;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateMentalLoad;

public class UpdateMentalLoadCommandValidator : AbstractValidator<UpdateMentalLoadCommand>
{
    public UpdateMentalLoadCommandValidator()
    {
        RuleFor(x => x.NewLoad)
            .IsInEnum()
            .WithMessage("Invalid mental load level.");
    }
}
