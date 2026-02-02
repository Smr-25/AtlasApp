using FluentValidation;

namespace Atlas.Application.Features.Workspaces.Commands.LinkIntegration;

public class LinkIntegrationCommandValidator : AbstractValidator<LinkIntegrationCommand>
{
    public LinkIntegrationCommandValidator()
    {
        RuleFor(x => x.WorkspaceId).NotEmpty().WithMessage("WorkspaceId is required.");
        RuleFor(x => x.IntegrationId).NotEmpty().WithMessage("IntegrationId is required.");
    }
}