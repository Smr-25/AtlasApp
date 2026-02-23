using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.SecOpsUtilities.Queries.CalculatePasswordEntropy;

public record CalculatePasswordEntropyQuery(string Password) : IRequest<PasswordEntropyResult>;

