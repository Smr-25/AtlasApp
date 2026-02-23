using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.LeaderScripts.Commands.RunSprintStarter;

public record RunSprintStarterCommand(string SprintName, List<string> InitialTasks, Guid TeamId) : IRequest<SprintStarterResult>;

