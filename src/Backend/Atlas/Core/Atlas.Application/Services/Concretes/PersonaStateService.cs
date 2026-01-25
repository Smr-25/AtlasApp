using Atlas.Application.Dtos.Users.State;
using Atlas.Application.Exceptions.Common;
using Atlas.Application.Interfaces;
using Atlas.Application.Models;
using Atlas.Application.Services.Interfaces;
using Atlas.Domain.Entities;
using AutoMapper;
using FluentValidation;
using ValidationException = Atlas.Application.Exceptions.Common.ValidationException;

namespace Atlas.Application.Services.Concretes;

public class PersonaStateService(
    IApplicationDbContext applicationDbContext,
    IMapper mapper,
    IValidator<PersonaStateCreateDto> personaStateCreateValidator) : IPersonaStateService
{
    public async Task<ResponseModel<PersonaStateReturnDto>> GetPersonaStateAsync(Guid userId)
    {
        var personaState = await applicationDbContext.PersonaStates.FindAsync(userId);
        if (personaState == null)
            throw new NotFoundException($"{nameof(PersonaState)} with id {userId} not found");

        var personaStateDto = mapper.Map<PersonaStateReturnDto>(personaState);
        var response = new ResponseModel<PersonaStateReturnDto>
        {
            Data = personaStateDto,
            IsSuccess = true,
            Errors = null
        };
        return response;
    }

    public async Task<ResponseModel<bool>> InitializePersonaStateAsync(PersonaStateCreateDto personaStateCreateDto)
    {
        var validationResult = await personaStateCreateValidator.ValidateAsync(personaStateCreateDto);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var personaState = mapper.Map<PersonaState>(personaStateCreateDto);
        await applicationDbContext.PersonaStates.AddAsync(personaState);
        await applicationDbContext.SaveChangesAsync();
        var response = new ResponseModel<bool>
        {
            Data = true,
            IsSuccess = true,
            Errors = null
        };
        return response;
    }

    public async Task<ResponseModel<bool>> UpdatePersonaStateAsync(Guid userId, PersonaStateUpdateDto personaStateUpdateDto)
    {
        var personaState = await applicationDbContext.PersonaStates.FindAsync(userId);
        if (personaState == null)
            throw new NotFoundException($"{nameof(PersonaState)} with id {userId} not found");

        mapper.Map(personaStateUpdateDto, personaState);
        applicationDbContext.PersonaStates.Update(personaState);
        await applicationDbContext.SaveChangesAsync();
        var response = new ResponseModel<bool>
        {
            Data = true,
            IsSuccess = true,
            Errors = null
        };
        return response;
    }
}