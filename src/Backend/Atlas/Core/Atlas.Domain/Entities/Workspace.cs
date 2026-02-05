using Atlas.Domain.Entities.Common;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Workspace : BaseEntity
{
    private readonly List<WorkspaceIntegration> _workspaceIntegrations = [];
    public Guid PersonaId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public bool IsDefault { get; private set; }
    public string? Config { get; private set; }
    public DateTimeOffset? LastAccessedAt { get; private set; }
    public Persona Persona { get; private set; } = null!;
    public IReadOnlyCollection<WorkspaceIntegration> WorkspaceIntegrations => _workspaceIntegrations.AsReadOnly();

    private Workspace()
    {
    }

    public static Workspace Create(
        Guid personaId,
        string name,
        string? description = null,
        string? icon = null,
        string? color = null,
        bool isDefault = false)
    {
        if (personaId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(Workspace), nameof(PersonaId),
                "Persona ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Name),
                "Workspace name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Name),
                "Workspace name cannot exceed 100 characters.");

        if (description?.Length > 500)
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Description),
                "Description cannot exceed 500 characters.");

        if (color != null && !IsValidHexColor(color))
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Color),
                "Color must be a valid hex color code (e.g., #FF5733).");

        return new Workspace
        {
            PersonaId = personaId,
            Name = name.Trim(),
            Description = description?.Trim(),
            Icon = icon?.Trim(),
            Color = color?.ToUpperInvariant(),
            IsDefault = isDefault
        };
    }

    public void Update(string name, string? description, string? icon, string? color)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Name),
                "Workspace name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Name),
                "Workspace name cannot exceed 100 characters.");

        if (description?.Length > 500)
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Description),
                "Description cannot exceed 500 characters.");

        if (color != null && !IsValidHexColor(color))
            throw new InvalidEntityStateException(nameof(Workspace), nameof(Color),
                "Color must be a valid hex color code (e.g., #FF5733).");

        Name = name.Trim();
        Description = description?.Trim();
        Icon = icon?.Trim();
        Color = color?.ToUpperInvariant();
        SetModified();
    }

    public void UpdateConfig(string? config)
    {
        Config = config;
        SetModified();
    }
    
    public void SetAsDefault()
    {
        IsDefault = true;
        SetModified();
    }
    
    public void RemoveDefaultStatus()
    {
        IsDefault = false;
        SetModified();
    }

    public void RecordAccess()
    {
        LastAccessedAt = DateTimeOffset.UtcNow;
        SetModified();
    }

    public void LinkIntegration(Guid integrationId, string? config = null)
    {
        if (integrationId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(WorkspaceIntegration), nameof(integrationId),
                "Integration ID cannot be empty.");

        if (_workspaceIntegrations.Any(wi => wi.IntegrationId == integrationId && !wi.IsDeleted))
        {
            throw new BusinessRuleViolationException(
                "DuplicateWorkspaceIntegration",
                "This integration is already linked to the workspace.");
        }

        var workspaceIntegration = WorkspaceIntegration.Create(Id, integrationId, config);
        _workspaceIntegrations.Add(workspaceIntegration);
        SetModified();
    }
    
    public void UnlinkIntegration(Guid integrationId)
    {
        var workspaceIntegration = _workspaceIntegrations
            .FirstOrDefault(wi => wi.IntegrationId == integrationId && !wi.IsDeleted);

        if (workspaceIntegration == null)
            throw new EntityNotFoundException("WorkspaceIntegration", integrationId);

        workspaceIntegration.Delete();
        SetModified();
    }
    
    private static bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        if (!color.StartsWith('#'))
            return false;

        var hex = color[1..];
        return hex.Length is 3 or 6 or 8 &&
               hex.All(char.IsAsciiHexDigit);
    }
}