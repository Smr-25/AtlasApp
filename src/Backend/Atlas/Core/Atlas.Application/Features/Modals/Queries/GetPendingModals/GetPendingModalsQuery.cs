using Atlas.Application.Features.Modals.Dtos;
using MediatR;

namespace Atlas.Application.Features.Modals.Queries.GetPendingModals;

public record GetPendingModalsQuery : IRequest<List<ModalStateDto>>;

