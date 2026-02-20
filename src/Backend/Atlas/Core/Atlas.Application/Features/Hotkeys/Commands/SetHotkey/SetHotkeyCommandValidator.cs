using FluentValidation;

namespace Atlas.Application.Features.Hotkeys.Commands.SetHotkey;

public class SetHotkeyCommandValidator : AbstractValidator<SetHotkeyCommand>
{
    public SetHotkeyCommandValidator()
    {
        RuleFor(x => x.Action)
            .NotEmpty().WithMessage("Action is required.")
            .MaximumLength(100).WithMessage("Action must not exceed 100 characters.");
        RuleFor(x => x.KeyCombination)
            .NotEmpty().WithMessage("Key combination is required.")
            .MaximumLength(100).WithMessage("Key combination must not exceed 100 characters.");
    }
}

