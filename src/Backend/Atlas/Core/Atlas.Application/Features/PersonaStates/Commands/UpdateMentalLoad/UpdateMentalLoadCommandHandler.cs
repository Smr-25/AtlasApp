using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonaStates.Commands.UpdateMentalLoad;

public class UpdateMentalLoadCommandHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateMentalLoadCommand, ResponseModel<PersonaStateDto>>
{
    public async Task<ResponseModel<PersonaStateDto>> Handle(UpdateMentalLoadCommand request,
        CancellationToken cancellationToken)
    {
        var personaState = await applicationDbContext.PersonaStates
            .FirstOrDefaultAsync(ps => ps.Persona.UserId.Equals(currentUserService.UserId), cancellationToken);

        if (personaState == null)
            throw new NotFoundException(nameof(PersonaState), currentUserService.UserId!);

        personaState.UpdateMentalLoad(request.NewLoad);

        await applicationDbContext.SaveChangesAsync();

        var dto = mapper.Map<PersonaStateDto>(personaState);
        return ResponseModel<PersonaStateDto>.Success(dto);
    }
}
