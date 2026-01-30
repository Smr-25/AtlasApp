using FluentValidation;

namespace Atlas.Application.Features.Reflections.Commands.UpdateReflection;

public class UpdateReflectionCommandValidator : AbstractValidator<UpdateReflectionCommand>
{
    public UpdateReflectionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Reflection ID is required.");
        RuleFor(x => x.Content).NotEmpty().When(x => x.Content is not null)
            .WithMessage("Content cannot be empty if provided.");
        RuleFor(x => x.MoodScore).InclusiveBetween(1, 10).When(x => x.MoodScore is not null)
            .WithMessage("Mood score must be between 1 and 10 if provided.");
    }
}