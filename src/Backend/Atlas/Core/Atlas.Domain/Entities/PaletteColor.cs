using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class PaletteColor : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string HexCode { get; private set; } = null!;
    public Guid PaletteId { get; private set; } 

    private PaletteColor() { }

    public static PaletteColor Create(string name, string hexCode)
    {
        return new PaletteColor
        {
            Id = Guid.NewGuid(),
            Name = name,
            HexCode = hexCode,
            CreatedAt = DateTime.UtcNow
        };
    }
}