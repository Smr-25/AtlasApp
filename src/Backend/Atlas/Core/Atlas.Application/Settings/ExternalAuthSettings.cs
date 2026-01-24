namespace Atlas.Application.Settings;

public class ExternalAuthSettings
{
    public AppleAuthSettings Apple { get; set; } = new();
    public GoogleAuthSettings Google { get; set; } = new();
}

public class AppleAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string TeamId { get; set; } = string.Empty;
    public string KeyId { get; set; } = string.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}

public class GoogleAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}

