using FluentValidation;

namespace Atlas.Application.Features.Scripts.Commands.RunScript;

public class RunScriptCommandValidator : AbstractValidator<RunScriptCommand>
{
    public RunScriptCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Script ID is required.");
    }
}
