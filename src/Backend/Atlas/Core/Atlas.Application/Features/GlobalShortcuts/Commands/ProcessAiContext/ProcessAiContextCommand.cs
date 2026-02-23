using Atlas.Application.Features.GlobalShortcuts.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.ProcessAiContext;

public record ProcessAiContextCommand(
    string SelectedContent,
    AiContextAction Action
) : IRequest<AiContextResultDto>;

