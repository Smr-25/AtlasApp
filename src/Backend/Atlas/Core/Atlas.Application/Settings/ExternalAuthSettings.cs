namespace Atlas.Application.Settings;

public class ExternalAuthSettings
{
    public const string SectionName = "ExternalAuthSettings";
    public GoogleAuthSettings Google { get; set; } = new();
    public GitHubAuthSettings GitHub { get; set; } = new();
}

public class GoogleAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string? FrontendRedirectUri { get; set; } = null;
}

public class GitHubAuthSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = "https://github.com/login/oauth/access_token";
    public string UserApiEndpoint { get; set; } = "https://api.github.com/user";
    public string UserEmailsEndpoint { get; set; } = "https://api.github.com/user/emails";
    public string? FrontendRedirectUri { get; set; } = null;
}
