namespace Atlas.Application.Common.Interfaces;

public interface IAiService
{
    Task<string> GenerateResponseAsync(string systemMessage, string userMessage, CancellationToken cancellationToken);
}