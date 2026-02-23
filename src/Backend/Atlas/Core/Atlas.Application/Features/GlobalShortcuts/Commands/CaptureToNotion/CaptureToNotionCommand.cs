using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.CaptureToNotion;

public record CaptureToNotionCommand(
    string Content,
    string? Title,
    string? Url
) : IRequest<Guid>;

