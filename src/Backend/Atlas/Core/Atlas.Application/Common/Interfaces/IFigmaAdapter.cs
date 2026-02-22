using Atlas.Application.Features.Figma.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface IFigmaAdapter
{
    Task<List<FigmaCommentDto>> GetCommentsAsync(string accessToken, string fileKey, CancellationToken ct);
    Task PostCommentAsync(string accessToken, string fileKey, string message, CancellationToken ct);
    Task ResolveCommentAsync(string accessToken, string fileKey, string commentId, CancellationToken ct);
    Task<List<FigmaComponentDto>> GetFileComponentsAsync(string accessToken, string fileKey, CancellationToken ct);
}

