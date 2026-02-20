using MediatR;

namespace Atlas.Application.Features.Focus.Commands.PauseFocusSession;

public record PauseFocusSessionCommand(Guid SessionId) : IRequest<bool>;

