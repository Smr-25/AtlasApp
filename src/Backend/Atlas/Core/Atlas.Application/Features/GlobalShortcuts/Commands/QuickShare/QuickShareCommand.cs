using Atlas.Application.Features.GlobalShortcuts.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.QuickShare;

public record QuickShareCommand(
    string Content,
    ShareChannel Channel,
    string? RecipientEmail,
    string? SlackChannel
) : IRequest<QuickShareResultDto>;

