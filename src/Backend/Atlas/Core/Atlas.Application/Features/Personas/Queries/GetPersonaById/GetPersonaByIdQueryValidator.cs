using FluentValidation;

namespace Atlas.Application.Features.Personas.Queries.GetPersonaById;

public class GetPersonaByIdQueryValidator : AbstractValidator<GetPersonaByIdQuery>
{
    public GetPersonaByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Persona ID is required.");
    }
}
