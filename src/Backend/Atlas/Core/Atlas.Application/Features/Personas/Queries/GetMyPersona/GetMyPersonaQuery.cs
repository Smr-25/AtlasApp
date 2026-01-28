using Atlas.Application.Common.Models;
using Atlas.Application.Features.Personas.Dtos;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.GetMyPersona;

public record GetMyPersonaQuery : IRequest<ResponseModel<PersonaDto>>;