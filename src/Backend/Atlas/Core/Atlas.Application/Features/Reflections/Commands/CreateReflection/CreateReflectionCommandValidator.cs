using FluentValidation;

namespace Atlas.Application.Features.Reflections.Commands.CreateReflection;

public class CreateReflectionCommandValidator : AbstractValidator<CreateReflectionCommand>
{
    public CreateReflectionCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(5000).WithMessage("Content cannot exceed 5000 characters.");
        
        RuleFor(x => x.ReflectionType)
            .IsInEnum().WithMessage("Invalid ReflectionType.");
        
        RuleFor(x => x.MoodScore)
            .InclusiveBetween(1, 10).When(x => x.MoodScore.HasValue)
            .WithMessage("MoodScore must be between 1 and 10.");

        RuleForEach(x => x.Tags)
            .MaximumLength(50).WithMessage("Each tag cannot exceed 50 characters.");
    }
}