using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Extensions;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Personas.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.Personas.Queries.GetPersonaById;

public class GetPersonaByIdQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetPersonaByIdQuery, PersonaDetailDto>
{
    public async Task<PersonaDetailDto> Handle(GetPersonaByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUserService.GetRequiredUserId();

        var entity = await context.Personas
            .Include(p => p.Integrations)
            .Include(p => p.Workspaces)
            .AsNoTracking() 
            .FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == userId, cancellationToken);

        if (entity == null)
            throw new NotFoundException("Persona", request.Id);
        

        return new PersonaDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Bio = entity.Bio,
            Type = entity.Type.ToString(), 
            IsPrimary = entity.IsPrimary,
            
            Integrations = entity.Integrations
                .Where(i => !i.IsDeleted)
                .Select(i => new PersonaIntegrationDto(
                    i.Id, 
                    i.Name, 
                    i.Provider.ToString(), 
                    true
                )).ToList(),
                
            Workspaces = entity.Workspaces
                .Where(w => !w.IsDeleted)
                .Select(w => new PersonaWorkspaceDto(
                    w.Id, 
                    w.Name
                )).ToList()
        };
    }
}