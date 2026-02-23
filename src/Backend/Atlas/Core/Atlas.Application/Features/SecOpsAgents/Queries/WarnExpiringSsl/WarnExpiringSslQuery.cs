using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsAgents.Queries.WarnExpiringSsl;

public record WarnExpiringSslQuery(List<string> Domains) : IRequest<List<ExpiringSslInfo>>;

