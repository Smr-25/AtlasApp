using Atlas.Application.Features.Focus.Dtos;
using MediatR;

namespace Atlas.Application.Features.Focus.Queries.GetFocusHistory;

public record GetFocusHistoryQuery(int Days = 7) : IRequest<List<FocusHistoryDto>>;

