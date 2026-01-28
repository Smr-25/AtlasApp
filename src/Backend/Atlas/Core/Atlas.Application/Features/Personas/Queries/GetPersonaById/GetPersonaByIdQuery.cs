using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.GetPersonaById;

public record GetPersonaByIdQuery(Guid PersonaId) : IRequest<ResponseModel<PersonaDto>>;