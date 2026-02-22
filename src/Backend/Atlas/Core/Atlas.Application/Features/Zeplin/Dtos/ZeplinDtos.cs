namespace Atlas.Application.Features.Zeplin.Dtos;

public record ZeplinScreenDto(
    string Id,
    string Name,
    string ImageUrl,
    int Width,
    int Height,
    DateTime UpdatedAt);

public record ZeplinStyleGuideDto(
    string ProjectId,
    List<ZeplinColorDto> Colors,
    List<ZeplinFontDto> Fonts,
    List<ZeplinSpacingDto> Spacings);

public record ZeplinColorDto(string Name, string HexCode, double Opacity);

public record ZeplinFontDto(string Family, double Size, string Weight);

public record ZeplinSpacingDto(string Name, double Value);

