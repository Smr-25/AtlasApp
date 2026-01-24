namespace Atlas.Application.Settings;

public class LockoutSettings
{
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public int LockoutDurationInMinutes { get; set; } = 15;
}