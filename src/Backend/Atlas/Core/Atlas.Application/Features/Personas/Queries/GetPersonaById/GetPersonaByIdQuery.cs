using Atlas.Application.Features.Personas.Dtos;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.GetPersonaById;

public record GetPersonaByIdQuery(Guid Id) : IRequest<PersonaDetailDto>;