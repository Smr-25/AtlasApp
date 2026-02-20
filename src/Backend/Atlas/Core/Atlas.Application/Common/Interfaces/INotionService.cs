using Atlas.Application.Features.Knowledge.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface INotionService
{
    Task<List<NoteDto>> GetImportantPagesAsync(string databaseId, string authToken, CancellationToken cancellationToken = default);
    Task<string> SendSnippetToNotionAsync(string title, string code, string language, string databaseId, string authToken, CancellationToken cancellationToken = default);
    Task<List<NotionSnippetDto>> FetchSnippetsFromNotionAsync(string databaseId, string authToken, int limit = 10, CancellationToken cancellationToken = default);
}

public record NotionSnippetDto(string Id, string Title, string Code, string Language, string Url, DateTime LastEdited);
