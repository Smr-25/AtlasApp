using MediatR;

namespace Atlas.Application.Features.Focus.Commands.InterruptFocusSession;

public record InterruptFocusSessionCommand(Guid SessionId) : IRequest<bool>;

