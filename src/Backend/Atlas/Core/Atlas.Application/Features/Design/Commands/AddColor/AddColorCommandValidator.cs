using FluentValidation;

namespace Atlas.Application.Features.Design.Commands.AddColor;

public class AddColorCommandValidator : AbstractValidator<AddColorCommand>
{
    public AddColorCommandValidator()
    {
        RuleFor(x => x.Name).
            NotEmpty()
            .WithMessage("Color name is required.")
            .MaximumLength(50)
            .WithMessage("Color name must not exceed 50 characters.");
        RuleFor(x => x.HexCode)
            .Matches("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Invalid HEX color format. Example: #FF5733");
    }
}