using Atlas.Application.Features.Miro.Dtos;
using MediatR;

namespace Atlas.Application.Features.Miro.Queries.GetBoards;

public record GetMiroBoardsQuery(Guid IntegrationId) : IRequest<List<MiroBoardDto>>;

