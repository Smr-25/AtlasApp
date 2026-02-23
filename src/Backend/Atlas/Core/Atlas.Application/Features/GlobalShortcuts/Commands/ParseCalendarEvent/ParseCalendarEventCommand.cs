using Atlas.Application.Features.GlobalShortcuts.Dtos;
using MediatR;

namespace Atlas.Application.Features.GlobalShortcuts.Commands.ParseCalendarEvent;

public record ParseCalendarEventCommand(string Text) : IRequest<CalendarEventResultDto>;

