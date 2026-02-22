using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.GenerateBoilerplate;

public record GenerateBoilerplateCommand(string ProjectName, string TemplateName, string OutputPath) : IRequest<string>;

