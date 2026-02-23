using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.GenerateRiskMatrix;

public record GenerateRiskMatrixCommand(List<RiskItem> Items) : IRequest<RiskMatrixResult>;

