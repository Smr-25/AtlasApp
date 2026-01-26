using Atlas.Application.Features.Personas.Dtos;
using Atlas.Application.Interfaces;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using AutoMapper;
using MediatR;

namespace Atlas.Application.Features.Personas.Commands.CreatePersona;

public class CreatePersonaCommandHandler(IApplicationDbContext applicationDbContext,IAccountService accountService,IMapper mapper) : IRequestHandler<CreatePersonaCommand,ResponseModel<PersonaDto>>
{
    public Task<ResponseModel<PersonaDto>> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}