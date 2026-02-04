using Atlas.Application.Features.Knowledge.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface INotionService
{
    Task<List<NoteDto>> GetImportantPagesAsync(string databaseId, string authToken, CancellationToken cancellationToken = default);
}