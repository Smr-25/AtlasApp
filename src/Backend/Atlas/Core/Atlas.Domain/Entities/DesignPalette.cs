using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class DesignPalette : BaseEntity
{
    public string Name { get; private set; } = null!;
    public Guid UserId { get; private set; }
    
    private readonly List<PaletteColor> _colors = [];
    public IReadOnlyCollection<PaletteColor> Colors => _colors.AsReadOnly();

    private DesignPalette() { }

    public static DesignPalette Create(Guid userId, string name)
    {
        return new DesignPalette
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void AddColor(string name, string hexCode)
    {
        var color = PaletteColor.Create(name, hexCode);
        _colors.Add(color);
        SetModified();
    }

    public void RemoveColor(Guid colorId)
    {
        var color = _colors.FirstOrDefault(c => c.Id == colorId);
        if (color == null) return;
        _colors.Remove(color);
        SetModified();
    }
}