using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.Focus.Commands.LogSession;

public record LogSessionCommand(
    int DurationMinutes,
    string Tag,
    FocusSessionType SessionType = FocusSessionType.Pomodoro,
    int BreakDurationMinutes = 5,
    Guid? WorkspaceId = null
) : IRequest<Guid>;
