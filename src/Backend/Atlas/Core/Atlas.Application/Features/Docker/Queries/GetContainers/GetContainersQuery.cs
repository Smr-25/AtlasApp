using Atlas.Application.Features.Docker.Dtos;
using MediatR;

namespace Atlas.Application.Features.Docker.Queries.GetContainers;

public record GetContainersQuery : IRequest<List<ContainerDto>>;