using Atlas.Application.Common.Interfaces;
using MediatR;

namespace Atlas.Application.Features.Scripts.Commands.GenerateBoilerplate;

public class GenerateBoilerplateCommandHandler(
    IAiService aiService,
    IScriptRunnerService scriptRunner
) : IRequestHandler<GenerateBoilerplateCommand, string>
{
    public async Task<string> Handle(GenerateBoilerplateCommand request, CancellationToken cancellationToken)
    {
        var prompt = $"Generate a folder structure and boilerplate code for a project named '{request.ProjectName}' using template '{request.TemplateName}'. Return only the shell commands to create directories and files.";
        var commands = await aiService.GenerateResponseAsync(
            "You are a code scaffolding assistant. Return only executable shell commands.",
            prompt, cancellationToken);

        var result = await scriptRunner.ExecuteAsync("bash", $"-c \"{commands}\"", request.OutputPath, cancellationToken);
        return result;
    }
}

