using MediatR;

namespace Atlas.Application.Features.Focus.Commands.LogSession;

public record LogSessionCommand(int DurationMinutes, string Tag) : IRequest<Guid>;