using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.CheckVpnDrop;

public record CheckVpnDropQuery : IRequest<VpnStatusResult>;

