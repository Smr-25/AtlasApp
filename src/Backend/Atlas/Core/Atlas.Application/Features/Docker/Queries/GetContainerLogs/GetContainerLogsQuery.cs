using MediatR;

namespace Atlas.Application.Features.Docker.Queries.GetContainerLogs;

public record GetContainerLogsQuery(
    string ContainerId,
    int TailCount = 100
) : IRequest<string>;