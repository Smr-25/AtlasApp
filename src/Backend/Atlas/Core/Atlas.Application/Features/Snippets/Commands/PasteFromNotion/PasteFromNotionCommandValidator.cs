using FluentValidation;

namespace Atlas.Application.Features.Snippets.Commands.PasteFromNotion;

public class PasteFromNotionCommandValidator : AbstractValidator<PasteFromNotionCommand>
{
    public PasteFromNotionCommandValidator()
    {
        RuleFor(x => x.NotionDatabaseId)
            .NotEmpty().WithMessage("Notion Database ID is required.");
        RuleFor(x => x.NotionAuthToken)
            .NotEmpty().WithMessage("Notion Auth Token is required.");
    }
}

