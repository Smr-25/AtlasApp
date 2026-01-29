using Atlas.Application.Common.Exceptions.Common;
using Atlas.Application.Common.Interfaces;
using Atlas.Application.Common.Models;
using Atlas.Domain.Entities;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Features.PersonaStates.Queries.GetStateHistory;

public class GetStateHistoryQueryHandler(IApplicationDbContext applicationDbContext,ICurrentUserService currentUserService, IMapper mapper) : IRequestHandler<GetStateHistoryQuery,ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetStateHistoryQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }   
}