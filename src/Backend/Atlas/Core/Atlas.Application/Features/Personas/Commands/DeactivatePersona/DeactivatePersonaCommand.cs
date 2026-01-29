using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.DeactivatePersona;

public record DeactivatePersonaCommand : IRequest<ResponseModel<bool>>;