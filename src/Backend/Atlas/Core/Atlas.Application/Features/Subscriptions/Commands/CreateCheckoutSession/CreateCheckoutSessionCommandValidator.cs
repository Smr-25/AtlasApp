using Atlas.Domain.Enums;
using FluentValidation;

namespace Atlas.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

public class CreateCheckoutSessionCommandValidator : AbstractValidator<CreateCheckoutSessionCommand>
{
    public CreateCheckoutSessionCommandValidator()
    {
        RuleFor(x => x.Tier)
            .IsInEnum().WithMessage("Invalid subscription tier.")
            .Must(t => t != SubscriptionTier.Free).WithMessage("Cannot create checkout for Free tier.");
        RuleFor(x => x.SuccessUrl)
            .NotEmpty().WithMessage("Success URL is required.");
        RuleFor(x => x.CancelUrl)
            .NotEmpty().WithMessage("Cancel URL is required.");
    }
}

