using MediatR;

namespace Atlas.Application.Features.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId) : IRequest;

