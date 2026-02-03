using Atlas.Domain.Entities.Common;

namespace Atlas.Domain.Entities;

public class Script : BaseEntity
{
    public string Name { get; set; } = null!;           
    public string Command { get; set; } = null!;        
    public string Arguments { get; set; } = null!;      
    public string? WorkingDirectory { get; set; }       
    public string? Icon { get; set; }                   
    public string? Color { get; set; }                  
    public Guid UserId { get; set; } 
    
    public static Script Create(string name, string command, string arguments, string? workingDirectory, string? icon, string? color, Guid userId)
    {
        return new Script
        {
            Id = Guid.NewGuid(),
            Name = name,
            Command = command,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            Icon = icon,
            Color = color,
            UserId = userId
        };
    }   
}