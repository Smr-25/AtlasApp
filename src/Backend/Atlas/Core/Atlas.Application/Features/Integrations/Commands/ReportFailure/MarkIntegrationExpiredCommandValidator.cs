using FluentValidation;

namespace Atlas.Application.Features.Integrations.Commands.ReportFailure;

public class MarkIntegrationExpiredCommandValidator : AbstractValidator<MarkIntegrationExpiredCommand>
{
    public MarkIntegrationExpiredCommandValidator()
    {
        RuleFor(x => x.IntegrationId)
            .NotEmpty()
            .WithMessage("IntegrationId is required.");
    }
}

