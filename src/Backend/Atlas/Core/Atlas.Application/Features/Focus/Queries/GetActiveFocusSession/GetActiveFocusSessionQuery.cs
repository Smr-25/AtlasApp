using Atlas.Application.Features.Focus.Dtos;
using MediatR;

namespace Atlas.Application.Features.Focus.Queries.GetActiveFocusSession;

public record GetActiveFocusSessionQuery : IRequest<FocusHistoryDto?>;

