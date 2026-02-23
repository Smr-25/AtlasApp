using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.CheckSsl;

public record CheckSslQuery(string Hostname) : IRequest<SslCheckResult>;

