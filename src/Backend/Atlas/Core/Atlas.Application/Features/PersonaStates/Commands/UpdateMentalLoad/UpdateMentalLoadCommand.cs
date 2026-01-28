using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Enums;
using MediatR;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateMentalLoad;

public record UpdateMentalLoadCommand(
    MentalLoadLevel NewLoad
) : IRequest<ResponseModel<PersonaStateDto>>;
