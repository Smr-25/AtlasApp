using MediatR;

namespace Atlas.Application.Features.Subscriptions.Commands.CancelSubscription;

public record CancelSubscriptionCommand : IRequest<bool>;

