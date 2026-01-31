using Atlas.Application.Common.Models;
using Atlas.Application.Features.Constraints.Dtos;
using MediatR;

namespace Atlas.Application.Features.Constraints.Queries.GetActiveConstraints;

public class GetActiveConstraintsQueryHandler : IRequestHandler<GetActiveConstraintsQuery, ResponseModel<List<ConstraintDto>>>
{
    public async Task<ResponseModel<List<ConstraintDto>>> Handle(GetActiveConstraintsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}