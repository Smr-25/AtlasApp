using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.MarketerAgents.Commands.WarnBudgetBleed;

public record WarnBudgetBleedCommand : IRequest<BudgetBleedResult>;

