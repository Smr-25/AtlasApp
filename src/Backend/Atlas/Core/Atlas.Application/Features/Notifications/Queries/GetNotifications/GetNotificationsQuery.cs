using Atlas.Application.Features.Notifications.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(
    NotificationCategory? Category = null,
    bool? UnreadOnly = null,
    int Page = 1,
    int PageSize = 30
) : IRequest<List<NotificationDto>>;

