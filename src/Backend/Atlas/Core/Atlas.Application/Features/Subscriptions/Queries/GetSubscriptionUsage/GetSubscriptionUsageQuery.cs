using Atlas.Application.Features.Subscriptions.Dtos;
using MediatR;

namespace Atlas.Application.Features.Subscriptions.Queries.GetSubscriptionUsage;

public record GetSubscriptionUsageQuery : IRequest<SubscriptionUsageDto>;

