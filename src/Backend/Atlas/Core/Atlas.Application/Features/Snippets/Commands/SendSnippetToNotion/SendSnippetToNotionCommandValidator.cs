using FluentValidation;

namespace Atlas.Application.Features.Snippets.Commands.SendSnippetToNotion;

public class SendSnippetToNotionCommandValidator : AbstractValidator<SendSnippetToNotionCommand>
{
    public SendSnippetToNotionCommandValidator()
    {
        RuleFor(x => x.SnippetId)
            .NotEmpty().WithMessage("Snippet ID is required.");
        RuleFor(x => x.NotionDatabaseId)
            .NotEmpty().WithMessage("Notion Database ID is required.");
        RuleFor(x => x.NotionAuthToken)
            .NotEmpty().WithMessage("Notion Auth Token is required.");
    }
}

