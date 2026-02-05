using Atlas.Domain.Abstractions;
using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Persona : BaseEntity, IAggregateRoot
{
    private readonly List<Integration> _integrations = [];
    private readonly List<Workspace> _workspaces = [];

    public Guid UserId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Bio { get; private set; }
    public PersonaType Type { get; private set; }
    public string? Config { get; private set; }
    public bool IsPrimary { get; private set; }
    public IReadOnlyCollection<Integration> Integrations => _integrations.AsReadOnly();
    public IReadOnlyCollection<Workspace> Workspaces => _workspaces.AsReadOnly();

    private Persona()
    {
    }

    public static Persona Create(
        Guid userId,
        string name,
        PersonaType type,
        string? bio = null,
        bool isPrimary = false)
    {
        if (userId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(Persona), nameof(UserId),
                "User ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Persona), nameof(Name),
                "Persona name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Persona), nameof(Name),
                "Persona name cannot exceed 100 characters.");

        if (bio?.Length > 500)
            throw new InvalidEntityStateException(nameof(Persona), nameof(Bio),
                "Bio cannot exceed 500 characters.");

        return new Persona
        {
            UserId = userId,
            Name = name.Trim(),
            Type = type,
            Bio = bio?.Trim(),
            IsPrimary = isPrimary
        };
    }

    public void UpdateProfile(string name, string? bio)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Persona), nameof(Name),
                "Persona name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Persona), nameof(Name),
                "Persona name cannot exceed 100 characters.");

        if (bio?.Length > 500)
            throw new InvalidEntityStateException(nameof(Persona), nameof(Bio),
                "Bio cannot exceed 500 characters.");

        Name = name.Trim();
        Bio = bio?.Trim();
        SetModified();
    }

    public void UpdateConfig(string? config)
    {
        Config = config;
        SetModified();
    }

    public void ChangeType(PersonaType newType)
    {
        if (Type == newType) return;

        Type = newType;
        Config = null;
        SetModified();
    }

    public void SetAsPrimary()
    {
        IsPrimary = true;
        SetModified();
    }

    public void RemovePrimaryStatus()
    {
        IsPrimary = false;
        SetModified();
    }

    public void AddIntegration(Integration integration)
    {
        ArgumentNullException.ThrowIfNull(integration);

        if (_integrations.Any(i => i.Provider == integration.Provider &&
                                   i.Name == integration.Name &&
                                   !i.IsDeleted))
        {
            throw new BusinessRuleViolationException(
                "DuplicateIntegration",
                $"An integration with provider '{integration.Provider.ToString()}' and name '{integration.Name}' already exists.");
        }

        _integrations.Add(integration);
        SetModified();
    }

    public void RemoveIntegration(Guid integrationId)
    {
        var integration = _integrations.FirstOrDefault(i => i.Id == integrationId);
        if (integration == null)
            throw new EntityNotFoundException(nameof(Integration), integrationId);

        integration.Delete();
        SetModified();
    }

    public void AddWorkspace(Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);

        if (_workspaces.Any(w => w.Name.Equals(workspace.Name, StringComparison.OrdinalIgnoreCase) &&
                                 !w.IsDeleted))
        {
            throw new BusinessRuleViolationException(
                "DuplicateWorkspace",
                $"A workspace named '{workspace.Name}' already exists.");
        }

        _workspaces.Add(workspace);
        SetModified();
    }

    public void RemoveWorkspace(Guid workspaceId)
    {
        var workspace = _workspaces.FirstOrDefault(w => w.Id == workspaceId);
        if (workspace == null)
            throw new EntityNotFoundException(nameof(Workspace), workspaceId);

        workspace.Delete();
        SetModified();
    }
}