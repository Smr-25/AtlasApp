namespace Atlas.Application.Features.LottieFiles.Dtos;

public record LottieAnimationDto(
    string Id,
    string Name,
    string PreviewUrl,
    string DownloadUrl,
    string AuthorName,
    int LikesCount);

