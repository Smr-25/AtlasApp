using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.DeactivePersona;

public record DeactivePersonaCommand : IRequest<ResponseModel<bool>>;