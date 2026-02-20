using FluentValidation;

namespace Atlas.Application.Features.Teams.Commands.CreateTeam;

public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
{
    public CreateTeamCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Team name is required.")
            .MinimumLength(2).WithMessage("Team name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Team name must not exceed 100 characters.");
    }
}

