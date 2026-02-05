using FluentValidation;

namespace Atlas.Application.Features.Design.Commands.CreatePalette;

public class CreatePaletteCommandValidator : AbstractValidator<CreatePaletteCommand>
{
    public CreatePaletteCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Palette name is required.")
            .MaximumLength(100).WithMessage("Palette name must not exceed 100 characters.");
    }
}