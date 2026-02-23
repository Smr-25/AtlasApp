using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderUtilities.Commands.GenerateQuickPoll;

public record GenerateQuickPollCommand(string Question, List<string> Options) : IRequest<QuickPollResult>;

