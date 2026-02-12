using FluentValidation;

namespace Atlas.Application.Features.JsonTools.Queries.FormatJson;

public class FormatJsonQueryValidator : AbstractValidator<FormatJsonQuery>
{
    public FormatJsonQueryValidator()
    {
        RuleFor(x => x.JsonContent)
            .NotEmpty().WithMessage("JSON content cannot be empty.");
    }
}