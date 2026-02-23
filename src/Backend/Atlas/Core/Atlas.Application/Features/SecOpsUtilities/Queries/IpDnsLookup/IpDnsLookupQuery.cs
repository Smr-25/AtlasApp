using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.IpDnsLookup;

public record IpDnsLookupQuery(string Target) : IRequest<IpDnsLookupResult>;

