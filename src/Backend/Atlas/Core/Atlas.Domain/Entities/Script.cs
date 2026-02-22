using Atlas.Domain.Entities.Common;
using Atlas.Domain.Enums;
using Atlas.Domain.Exceptions;

namespace Atlas.Domain.Entities;

public class Script : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Command { get; private set; } = null!;
    public string Arguments { get; private set; } = null!;
    public string? WorkingDirectory { get; private set; }
    public string? Icon { get; private set; }
    public string? Color { get; private set; }
    public ScriptType ScriptType { get; private set; }
    public Guid UserId { get; private set; }

    private Script() { }

    public static Script Create(string name, string command, string arguments,
        string? workingDirectory, string? icon, string? color, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Script), nameof(Name),
                "Script name cannot be empty.");

        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Script), nameof(Name),
                "Script name cannot exceed 100 characters.");

        if (string.IsNullOrWhiteSpace(command))
            throw new InvalidEntityStateException(nameof(Script), nameof(Command),
                "Command cannot be empty.");

        if (userId == Guid.Empty)
            throw new InvalidEntityStateException(nameof(Script), nameof(UserId),
                "User ID cannot be empty.");

        return new Script
        {
            Name = name.Trim(),
            Command = command.Trim(),
            Arguments = arguments?.Trim() ?? string.Empty,
            WorkingDirectory = workingDirectory?.Trim(),
            Icon = icon?.Trim(),
            Color = color?.ToUpperInvariant(),
            UserId = userId
        };
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidEntityStateException(nameof(Script), nameof(Name),
                "Script name cannot be empty.");
        
        if (name.Length > 100)
            throw new InvalidEntityStateException(nameof(Script), nameof(Name),
                "Script name cannot exceed 100 characters.");
        
        Name = name.Trim();
        SetModified();
    }

    public void UpdateCommand(string command, string? arguments = null)
    {
        if (string.IsNullOrWhiteSpace(command))
            throw new InvalidEntityStateException(nameof(Script), nameof(Command),
                "Command cannot be empty.");

        Command = command.Trim();
        if (arguments != null)
            Arguments = arguments.Trim();
        SetModified();
    }

    public void UpdateWorkingDirectory(string? workingDirectory)
    {
        WorkingDirectory = workingDirectory?.Trim();
        SetModified();
    }

    public void UpdateAppearance(string? icon, string? color)
    {
        Icon = icon?.Trim();
        Color = color?.ToUpperInvariant();
        SetModified();
    }
}