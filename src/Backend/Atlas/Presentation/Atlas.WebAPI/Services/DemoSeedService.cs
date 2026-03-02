using Atlas.Application.Common.Interfaces;
using Atlas.Domain.Entities;
using Atlas.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atlas.WebAPI.Services;

public static class DemoSeedService
{
    public static async Task SeedDemoDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var anyUser = await db.UserProfiles.AnyAsync();
        if (!anyUser)
        {
            logger.LogInformation("No users found — skipping demo seed (register first, then restart).");
            return;
        }

        var firstProfile = await db.UserProfiles.FirstAsync();
        var userId = firstProfile.Id;

        logger.LogInformation("Seeding demo data for user {UserId}...", userId);

        if (!await db.FocusSessions.AnyAsync(f => f.UserId == userId))
        {
            var rng = new Random(42);

            for (int day = 0; day < 14; day++)
            {
                var sessionsPerDay = rng.Next(1, 4);
                for (int s = 0; s < sessionsPerDay; s++)
                {
                    var duration = rng.Next(15, 50);
                    var tag = day % 2 == 0 ? "Deep Work" : "Code Review";

                    var session = FocusSession.Create(duration, tag, userId, FocusSessionType.DeepWork);
                    session.Complete();

                    await db.FocusSessions.AddAsync(session);
                }
            }

            logger.LogInformation("Seeded focus sessions");
        }

        if (!await db.UserActivities.AnyAsync(a => a.UserId == userId))
        {
            string[] scripts = ["docker-cleanup", "git-sync", "lint-fix", "db-migrate", "cache-flush"];

            for (int i = 0; i < 30; i++)
            {
                var actionType = i % 5 == 4 ? "ContextSwitch" : "ScriptRun";
                var desc = actionType == "ScriptRun"
                    ? $"Executed {scripts[i % scripts.Length]} script"
                    : "Switched from IDE to browser";

                var activity = UserActivity.Create(userId, actionType, desc);
                await db.UserActivities.AddAsync(activity);
            }

            logger.LogInformation("Seeded user activities");
        }

        if (!await db.Notifications.AnyAsync(n => n.UserId == userId))
        {
            var notifications = new[]
            {
                Notification.Create(userId, NotificationCategory.AlertsSecOps, NotificationPriority.Critical,
                    "Rogue Port Detected",
                    "Port 8080 is open on your dev machine and exposed to the network.",
                    "KillPort", "{\"port\": 8080, \"endpoint\": \"/api/system-tools/check-port/8080\"}"),
                Notification.Create(userId, NotificationCategory.AlertsSecOps, NotificationPriority.High,
                    "SSL Certificate Expiring",
                    "Certificate for api.myapp.com expires in 7 days.",
                    "RenewSsl", "{\"domain\": \"api.myapp.com\"}"),

                Notification.Create(userId, NotificationCategory.ApprovalsGit, NotificationPriority.Normal,
                    "New PR: Fix login validation bug",
                    "Sarah opened PR #142 in atlas-frontend repo. 3 files changed.",
                    "ApprovePr", "{\"prNumber\": 142, \"repo\": \"atlas-frontend\"}"),
                Notification.Create(userId, NotificationCategory.ApprovalsGit, NotificationPriority.Normal,
                    "PR Approved: Add dark mode support",
                    "Alex approved your PR #138. Ready to merge.",
                    "MergePr", "{\"prNumber\": 138, \"repo\": \"atlas-ui\"}"),

                Notification.Create(userId, NotificationCategory.MentionsSocial, NotificationPriority.Low,
                    "Figma Comment: Button color question",
                    "Designer asks: 'Should the CTA button be primary blue or accent green?'",
                    "ReplyComment", "{\"fileKey\": \"abc123\", \"commentId\": \"c456\"}"),
                Notification.Create(userId, NotificationCategory.MentionsSocial, NotificationPriority.Low,
                    "Team Mention in Standup",
                    "You were mentioned in the daily standup thread by team lead."),

                Notification.Create(userId, NotificationCategory.SystemInsights, NotificationPriority.Normal,
                    "Weekly Focus Report Ready",
                    "You focused for 18.5 hours this week — 23% improvement over last week!",
                    "ViewReport", "{\"route\": \"/focus\"}"),
                Notification.Create(userId, NotificationCategory.SystemInsights, NotificationPriority.Normal,
                    "3 Dependencies Have Updates",
                    "React, TypeScript, and Vite have new major versions available.",
                    "ViewDeps", "{\"route\": \"/dev-utilities\"}"),
            };

            await db.Notifications.AddRangeAsync(notifications);
            logger.LogInformation("Seeded {Count} notifications", notifications.Length);
        }

        if (!await db.InsightSnapshots.AnyAsync(s => s.UserId == userId))
        {
            for (int week = 0; week < 4; week++)
            {
                await db.InsightSnapshots.AddAsync(
                    InsightSnapshot.Create(userId, InsightType.TimeSaved, "TimeSaved", 2.5 + week * 0.8, "hours"));
                await db.InsightSnapshots.AddAsync(
                    InsightSnapshot.Create(userId, InsightType.DeploySuccess, "DeploymentSuccessRate", 85.0 + week * 3, "%"));
            }

            logger.LogInformation("Seeded insight snapshots");
        }

        await db.SaveChangesAsync(CancellationToken.None);
        logger.LogInformation("Demo seed completed successfully.");
    }
}
