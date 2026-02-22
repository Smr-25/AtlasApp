namespace Atlas.Application.Features.Perplexity.Dtos;

public record PerplexitySearchResultDto(
    string Answer,
    List<string> Sources,
    string Query);

