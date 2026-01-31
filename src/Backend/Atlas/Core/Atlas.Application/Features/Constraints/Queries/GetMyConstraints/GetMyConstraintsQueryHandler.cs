using Atlas.Application.Common.Models;
using MediatR;

namespace Atlas.Application.Features.Constraints.Queries.GetMyConstraints;

public class GetMyConstraintsQueryHandler : IRequestHandler<GetMyConstraintsQuery, ResponseModel<PagedResult>>
{
    public async Task<ResponseModel<PagedResult>> Handle(GetMyConstraintsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}