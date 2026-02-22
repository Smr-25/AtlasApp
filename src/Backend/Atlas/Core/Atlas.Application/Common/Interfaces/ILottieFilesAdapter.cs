using Atlas.Application.Features.LottieFiles.Dtos;

namespace Atlas.Application.Common.Interfaces;

public interface ILottieFilesAdapter
{
    Task<List<LottieAnimationDto>> SearchAnimationsAsync(string accessToken, string query, CancellationToken ct);
    Task<byte[]> DownloadAnimationAsync(string accessToken, string animationId, CancellationToken ct);
}

