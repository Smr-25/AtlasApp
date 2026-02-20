using Atlas.Application.Features.Subscriptions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Subscriptions.Queries.GetCurrentSubscription;

public record GetCurrentSubscriptionQuery : IRequest<SubscriptionDto>;

