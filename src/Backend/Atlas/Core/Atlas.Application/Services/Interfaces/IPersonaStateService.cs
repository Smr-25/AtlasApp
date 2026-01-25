using Atlas.Application.Dtos.Users.State;
using Atlas.Application.Models;

namespace Atlas.Application.Services.Interfaces;

public interface IPersonaStateService
{
    Task<ResponseModel<PersonaStateReturnDto>> GetPersonaStateAsync(Guid userId);
    Task<ResponseModel<bool>> InitializePersonaStateAsync(PersonaStateCreateDto personaStateCreateDto);
    Task<ResponseModel<bool>> UpdatePersonaStateAsync(Guid userId, PersonaStateUpdateDto personaStateUpdateDto);
}