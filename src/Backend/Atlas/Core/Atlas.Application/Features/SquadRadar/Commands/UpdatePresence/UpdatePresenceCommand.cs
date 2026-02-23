using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.SquadRadar.Commands.UpdatePresence;

public record UpdatePresenceCommand(Guid TeamId, SquadMemberStatus Status, string? ToolIcon, string? Focus, int? MeetingMinutesLeft) : IRequest<Unit>;

