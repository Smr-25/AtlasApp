namespace Atlas.Application.Features.Search.Dtos;

public record SearchResultDto(
    string Type,
    Guid Id,
    string Title,
    string? Subtitle,
    string? Icon,
    string? Route
);

public record GlobalSearchResultDto(
    List<SearchResultDto> Workspaces,
    List<SearchResultDto> Integrations,
    List<SearchResultDto> Scripts,
    List<SearchResultDto> Snippets,
    List<SearchResultDto> Projects,
    List<SearchResultDto> Teams,
    List<SearchResultDto> Commands
);

