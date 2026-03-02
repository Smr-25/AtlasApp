using Atlas.Application.Features.Notifications.Dtos;
using MediatR;

namespace Atlas.Application.Features.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<NotificationCountDto>;

