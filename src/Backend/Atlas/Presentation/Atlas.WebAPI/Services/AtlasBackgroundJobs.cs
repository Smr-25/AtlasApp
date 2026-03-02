using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Atlas.WebAPI.Services;
public class AtlasBackgroundJobs(IServiceScopeFactory scopeFactory, ILogger<AtlasBackgroundJobs> logger)
{
    
    public async Task SystemHealthCheckAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var monitor = scope.ServiceProvider.GetRequiredService<ISystemMonitorService>();
        var aiAdvisor = scope.ServiceProvider.GetRequiredService<IAiAdvisorService>();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var hub = scope.ServiceProvider.GetRequiredService<IAtlasHubService>();

        try
        {
            var snapshot = await monitor.GetSnapshotAsync();
            logger.LogInformation("[HealthCheck] CPU: {Cpu}%, RAM: {Ram}GB/{Total}GB, Battery: {Batt}%",
                snapshot.CpuLoad, snapshot.MemoryUsedGb, snapshot.TotalMemoryGb, snapshot.BatteryPercentage);

            var alerts = new List<(string title, string body, NotificationPriority prio)>();

            var ramPercent = snapshot.TotalMemoryGb > 0
                ? (snapshot.MemoryUsedGb / snapshot.TotalMemoryGb) * 100
                : 0;
            if (ramPercent > 85)
                alerts.Add(("High Memory Usage", $"RAM is at {ramPercent:F0}% ({snapshot.MemoryUsedGb:F1}GB/{snapshot.TotalMemoryGb:F1}GB). Consider closing unused apps.", NotificationPriority.High));

            if (snapshot.CpuLoad > 90)
                alerts.Add(("CPU Overload", $"CPU load is at {snapshot.CpuLoad:F0}%. Top process: {snapshot.TopProcesses.FirstOrDefault()?.Name ?? "unknown"}", NotificationPriority.Critical));

            if (snapshot.BatteryPercentage < 20 && snapshot.BatteryPercentage > 0)
                alerts.Add(("Low Battery Warning", $"Battery at {snapshot.BatteryPercentage}%. Estimated {snapshot.RemainingMinutes} minutes remaining.", NotificationPriority.High));

            if (alerts.Count > 0)
            {
                var advice = await aiAdvisor.AnalyzeHealthAsync(snapshot);
                if (advice.IsCritical)
                    alerts.Add(("AI System Advisor", advice.ActionableAdvice, NotificationPriority.Critical));
                else
                    alerts.Add(("AI System Advisor", advice.Summary, NotificationPriority.Normal));
            }

            var userIds = await db.UserProfiles.Select(u => u.Id).ToListAsync();

            foreach (var userId in userIds)
            {
                foreach (var (title, body, prio) in alerts)
                {
                    var recentExists = await db.Notifications
                        .AnyAsync(n => n.UserId == userId && n.Title == title
                                       && n.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-30));
                    if (recentExists) continue;

                    var notification = Notification.Create(
                        userId, NotificationCategory.SystemInsights, prio,
                        title, body, "ViewSystem", "{\"route\": \"/system\"}");

                    await db.Notifications.AddAsync(notification);

                    await hub.SendToUserAsync(userId, "NotificationReceived", new
                    {
                        Id = notification.Id,
                        Title = title,
                        Body = body,
                        Category = "SystemInsights",
                        Priority = prio.ToString(),
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            await db.SaveChangesAsync(CancellationToken.None);
            logger.LogInformation("[HealthCheck] Completed. {AlertCount} alerts generated.", alerts.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[HealthCheck] Failed");
        }
    }

    
    public async Task DockerHealthCheckAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var docker = scope.ServiceProvider.GetRequiredService<IDockerAdapter>();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var hub = scope.ServiceProvider.GetRequiredService<IAtlasHubService>();

        try
        {
            var containers = await docker.GetContainersAsync(CancellationToken.None);
            var unhealthy = containers.Where(c =>
                c.State.Equals("exited", StringComparison.OrdinalIgnoreCase) ||
                c.Status.Contains("unhealthy", StringComparison.OrdinalIgnoreCase)).ToList();

            if (unhealthy.Count == 0)
            {
                logger.LogInformation("[DockerCheck] All containers healthy. Total: {Count}", containers.Count);
                return;
            }

            var names = string.Join(", ", unhealthy.Select(c => c.Name));
            var body = $"{unhealthy.Count} container(s) need attention: {names}";

            var userIds = await db.UserProfiles.Select(u => u.Id).ToListAsync();
            foreach (var userId in userIds)
            {
                var recentExists = await db.Notifications
                    .AnyAsync(n => n.UserId == userId && n.Title == "Docker Container Alert"
                                   && n.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-30));
                if (recentExists) continue;

                var notification = Notification.Create(
                    userId, NotificationCategory.AlertsSecOps, NotificationPriority.High,
                    "Docker Container Alert", body,
                    "ViewDocker", "{\"route\": \"/docker\"}");

                await db.Notifications.AddAsync(notification);
                await hub.SendToUserAsync(userId, "NotificationReceived", new
                {
                    Id = notification.Id,
                    Title = "Docker Container Alert",
                    Body = body,
                    Category = "AlertsSecOps",
                    Priority = "High"
                });
            }

            await db.SaveChangesAsync(CancellationToken.None);
            logger.LogInformation("[DockerCheck] {Count} unhealthy containers detected.", unhealthy.Count);
        }
        catch (Exception ex)
        {
            logger.LogWarning("[DockerCheck] Docker not available: {Message}", ex.Message);
        }
    }

    public async Task DailyInsightsAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var hub = scope.ServiceProvider.GetRequiredService<IAtlasHubService>();

        try
        {
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var userIds = await db.UserProfiles.Select(u => u.Id).ToListAsync();

            foreach (var userId in userIds)
            {
                var sessions = await db.FocusSessions
                    .Where(f => f.UserId == userId && f.CompletedAt != null
                                && f.StartedAt >= yesterday)
                    .ToListAsync();

                if (sessions.Count > 0)
                {
                    var totalMinutes = sessions.Sum(s => s.DurationMinutes);
                    var body = $"Yesterday: {sessions.Count} focus sessions, {totalMinutes} minutes total. " +
                              $"Most productive: {sessions.OrderByDescending(s => s.DurationMinutes).First().Tag}";

                    var notification = Notification.Create(
                        userId, NotificationCategory.SystemInsights, NotificationPriority.Normal,
                        "Daily Focus Report", body,
                        "ViewFocus", "{\"route\": \"/focus\"}");

                    await db.Notifications.AddAsync(notification);
                    await hub.SendToUserAsync(userId, "NotificationReceived", new
                    {
                        notification.Id, Title = "Daily Focus Report", Body = body,
                        Category = "SystemInsights", Priority = "Normal"
                    });
                }
            }

            await db.SaveChangesAsync(CancellationToken.None);
            logger.LogInformation("[DailyInsights] Daily reports generated for {Count} users.", userIds.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[DailyInsights] Failed");
        }
    }
}
