using Atlas.Application.Features.Personas.Dtos;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.GetPersonas;

public record GetPersonasQuery : IRequest<List<PersonaDto>>;