using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Subscriptions.Commands.CreateCheckoutSession;

public record CreateCheckoutSessionCommand(
    SubscriptionTier Tier,
    string SuccessUrl,
    string CancelUrl
) : IRequest<string>;

