using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Queries.GetCurrentState;

public record GetCurrentStateQuery : IRequest<ResponseModel<PersonaStateDto>>;