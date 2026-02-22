
using MediatR;

namespace Atlas.Application.Features.DevUtilities.Queries.DecodeJwt;

public record DecodeJwtQuery(string Token) : IRequest<DecodeJwtResult>;

public record DecodeJwtResult(string Header, string Payload, DateTime? ExpiresAt, bool IsExpired);

