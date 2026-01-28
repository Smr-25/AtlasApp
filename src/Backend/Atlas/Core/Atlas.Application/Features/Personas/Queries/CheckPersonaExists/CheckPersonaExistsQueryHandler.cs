using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Personas.Queries.CheckPersonaExists;

public class CheckPersonaExistsQueryHandler(
    IApplicationDbContext applicationDbContext,
    ICurrentUserService currentUserService)
    : IRequestHandler<CheckPersonaExistsQuery, ResponseModel<bool>>
{
    public async Task<ResponseModel<bool>> Handle(CheckPersonaExistsQuery request, CancellationToken cancellationToken)
    {
        var persona = await applicationDbContext.Personas
            .FindAsync([currentUserService.UserId], cancellationToken);
        var exists = persona != null;
        return ResponseModel<bool>.Success(exists);
    }
}