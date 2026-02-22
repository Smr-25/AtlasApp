namespace Atlas.Application.Features.Dribbble.Dtos;

public record DribbbleShotDto(
    string Id,
    string Title,
    string HtmlUrl,
    string ImageUrl,
    string AuthorName,
    string AuthorAvatarUrl,
    int LikesCount,
    int ViewsCount,
    DateTime PublishedAt);

