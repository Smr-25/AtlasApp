using Atlas.Application.Features.Miro.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IMiroAdapter
{
    Task<List<MiroBoardDto>> GetBoardsAsync(string accessToken, CancellationToken ct);
    Task CreateStickyNoteAsync(string accessToken, string boardId, string content, CancellationToken ct);
}

