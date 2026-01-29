using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.ActivatePersona;

public record ActivatePersonaCommand : IRequest<ResponseModel<bool>>;