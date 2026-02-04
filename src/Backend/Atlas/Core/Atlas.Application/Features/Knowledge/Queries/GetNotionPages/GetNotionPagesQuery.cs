using Atlas.Application.Features.Knowledge.Dtos;
using MediatR;

namespace Atlas.Application.Features.Knowledge.Queries.GetNotionPages;

public record GetNotionPagesQuery : IRequest<List<NoteDto>>;