namespace Atlas.Application.Features.Design.Dtos;

public record DesignPaletteDto(Guid Id, string Name, List<PaletteColorDto> Colors);