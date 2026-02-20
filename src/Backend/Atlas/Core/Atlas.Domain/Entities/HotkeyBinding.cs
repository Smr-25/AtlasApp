using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class HotkeyBinding : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string KeyCombination { get; private set; } = null!;
    public bool IsGlobal { get; private set; }
    public bool IsEnabled { get; private set; }

    private HotkeyBinding() { }

    public static HotkeyBinding Create(Guid userId, string action, string keyCombination, bool isGlobal = false)
    {
        return new HotkeyBinding
        {
            UserId = userId,
            Action = action,
            KeyCombination = keyCombination,
            IsGlobal = isGlobal,
            IsEnabled = true
        };
    }

    public static HotkeyBinding CreateDefault(Guid userId, string action, string keyCombination, bool isGlobal)
    {
        return new HotkeyBinding
        {
            UserId = userId,
            Action = action,
            KeyCombination = keyCombination,
            IsGlobal = isGlobal,
            IsEnabled = true
        };
    }

    public void UpdateKeyCombination(string keyCombination)
    {
        KeyCombination = keyCombination;
        SetModified();
    }

    public void ToggleEnabled()
    {
        IsEnabled = !IsEnabled;
        SetModified();
    }

    public void Delete()
    {
        SetDelete();
    }
}

