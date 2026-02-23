using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunMeetingMode;

public record RunMeetingModeCommand(int DurationMinutes) : IRequest<string>;

