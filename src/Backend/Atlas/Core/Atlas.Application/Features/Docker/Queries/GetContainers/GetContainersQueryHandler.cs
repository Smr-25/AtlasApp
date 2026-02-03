using Atlas.Application.Common.Interfaces;
using Atlas.Application.Features.Docker.Dtos;
using MediatR;

namespace Atlas.Application.Features.Docker.Queries.GetContainers;

public class GetContainersQueryHandler(IDockerService dockerService)
    : IRequestHandler<GetContainersQuery, List<ContainerDto>>
{
    public async Task<List<ContainerDto>> Handle(GetContainersQuery request, CancellationToken cancellationToken)
    {
        return await dockerService.GetContainersAsync(cancellationToken);
    }
}