using FluentValidation;

namespace Atlas.Application.Features.Scripts.Commands.CreateScript;

public class CreateScriptCommandValidator : AbstractValidator<CreateScriptCommand>
{
    public CreateScriptCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Script name is required.")
            .MaximumLength(100).WithMessage("Script name cannot exceed 100 characters.");

        RuleFor(x => x.Command)
            .NotEmpty().WithMessage("Command is required.")
            .MaximumLength(500).WithMessage("Command cannot exceed 500 characters.");

        RuleFor(x => x.Arguments)
            .MaximumLength(1000).WithMessage("Arguments cannot exceed 1000 characters.");

        RuleFor(x => x.WorkingDirectory)
            .MaximumLength(500).WithMessage("Working directory path cannot exceed 500 characters.");
    }
}
