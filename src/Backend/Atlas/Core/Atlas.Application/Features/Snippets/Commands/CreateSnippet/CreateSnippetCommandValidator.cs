using FluentValidation;

namespace Atlas.Application.Features.Snippets.Commands.CreateSnippet;

public class CreateSnippetCommandValidator : AbstractValidator<CreateSnippetCommand>
{
    public CreateSnippetCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Snippet title is required.")
            .MaximumLength(200).WithMessage("Snippet title cannot exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.");

        RuleFor(x => x.Language)
            .MaximumLength(50).WithMessage("Language cannot exceed 50 characters.");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count <= 10)
            .WithMessage("Cannot have more than 10 tags.");
    }
}
