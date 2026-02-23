using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Queries.DetectCartAbandonment;

public record DetectCartAbandonmentQuery : IRequest<CartAbandonmentResult>;

