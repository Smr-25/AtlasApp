namespace Atlas.Application.Settings;

public class RateLimitSettings
{
    public RateLimitPolicy Fixed { get; set; } = new() { PermitLimit = 100, WindowInSeconds = 60 };
    public RateLimitPolicy Login { get; set; } = new() { PermitLimit = 5, WindowInSeconds = 60 };
    public RateLimitPolicy Register { get; set; } = new() { PermitLimit = 10, WindowInSeconds = 3600 };
    public RateLimitPolicy PasswordReset { get; set; } = new() { PermitLimit = 3, WindowInSeconds = 3600 };
    public RateLimitPolicy Verification { get; set; } = new() { PermitLimit = 5, WindowInSeconds = 60 };
    public RateLimitPolicy Resend { get; set; } = new() { PermitLimit = 5, WindowInSeconds = 3600 };
    public RateLimitPolicy Api { get; set; } = new() { PermitLimit = 60, WindowInSeconds = 60, SegmentsPerWindow = 6 };
}

public class RateLimitPolicy
{
    public int PermitLimit { get; set; }
    public int WindowInSeconds { get; set; }
    public int SegmentsPerWindow { get; set; } = 1;
}
