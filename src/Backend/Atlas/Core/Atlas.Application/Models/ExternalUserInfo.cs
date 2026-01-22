namespace Atlas.Application.Models;

public class ExternalUserInfo
{
    public string ProviderKey { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; } 
    public string? LastName { get; set; }
}