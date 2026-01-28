using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Application.Features.PersonaStates.Dtos;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonaStates.Queries.GetCurrentState;

public class GetCurrentStateQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetCurrentStateQuery, ResponseModel<PersonaStateDto>>
{
    public async Task<ResponseModel<PersonaStateDto>> Handle(GetCurrentStateQuery request,
        CancellationToken cancellationToken)
    {
        var personaState = await applicationDbContext.PersonaStates
            .FirstOrDefaultAsync(ps => ps.Persona.UserId.Equals(currentUserService.UserId), cancellationToken);

        if (personaState == null)
            return ResponseModel<PersonaStateDto>.Failure("Persona state not found.");


        var dto = mapper.Map<PersonaStateDto>(personaState);
        return ResponseModel<PersonaStateDto>.Success(dto);
    }
}