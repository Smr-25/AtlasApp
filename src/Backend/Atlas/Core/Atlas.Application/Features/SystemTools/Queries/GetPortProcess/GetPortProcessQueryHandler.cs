using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.SystemTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.SystemTools.Queries.GetPortProcess;

public class GetPortProcessQueryHandler(ISystemToolAdapter systemTool) 
    : IRequestHandler<GetPortProcessQuery, ProcessInfoDto>
{
    public async Task<ProcessInfoDto> Handle(GetPortProcessQuery request, CancellationToken cancellationToken)
    {
        return await systemTool.GetProcessByPortAsync(request.Port, cancellationToken);
    }
}