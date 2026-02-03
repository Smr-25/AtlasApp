using Atlas.Application.Features.System.Dtos;
using MediatR;

namespace Atlas.Application.Features.System.Queries.GetActiveIdes;

public record GetActiveIdesQuery : IRequest<List<IdeStatusDto>>;