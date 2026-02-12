using Atlas.Application.Features.SystemTools.Dtos;
using MediatR;

namespace Atlas.Application.Features.SystemTools.Queries.GetPortProcess;

public record GetPortProcessQuery(int Port) : IRequest<ProcessInfoDto>;