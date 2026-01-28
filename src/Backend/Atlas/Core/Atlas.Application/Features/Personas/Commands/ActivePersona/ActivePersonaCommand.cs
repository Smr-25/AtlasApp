using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.ActivePersona;

public record ActivePersonaCommand : IRequest<ResponseModel<bool>>;