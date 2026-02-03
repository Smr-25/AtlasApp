namespace Atlas.Application.Common.Interfaces;

public interface IScriptRunnerService
{
    Task<string> ExecuteAsync(string command, string arguments, string workingDirectory, CancellationToken cancellationToken = default);
}