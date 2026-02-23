using Atlas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<AppUserProfile> UserProfiles { get; }
    DbSet<Integration> Integrations { get; }
    DbSet<Workspace> Workspaces { get; }
    DbSet<WorkspaceIntegration> WorkspaceIntegrations { get; }
    DbSet<OnboardingQuestion> OnboardingQuestions { get; }
    DbSet<OnboardingOption> OnboardingOptions { get; }
    DbSet<OnboardingAnswer> OnboardingAnswers { get; }
    DbSet<UserActivity> UserActivities { get; }
    DbSet<Snippet> Snippets { get; }
    DbSet<Script> Scripts { get; }
    DbSet<FocusSession> FocusSessions { get; }
    DbSet<ProjectProfile> ProjectProfiles { get; }
    DbSet<DesignAsset> DesignAssets { get; }
    DbSet<DesignPalette> DesignPalettes { get; }
    DbSet<PaletteColor> PaletteColors { get; }
    DbSet<Subscription> Subscriptions { get; }
    DbSet<Team> Teams { get; }
    DbSet<TeamMember> TeamMembers { get; }
    DbSet<ModalState> ModalStates { get; }
    DbSet<HotkeyBinding> HotkeyBindings { get; }
    DbSet<SentryIssue> SentryIssues { get; }
    DbSet<AwsDeployment> AwsDeployments { get; }
    DbSet<SonarQubeReport> SonarQubeReports { get; }
    DbSet<ProactiveAlert> ProactiveAlerts { get; }
    DbSet<DependencyWatch> DependencyWatches { get; }
    DbSet<FigmaComment> FigmaComments { get; }
    DbSet<DesignHandoff> DesignHandoffs { get; }
    DbSet<DesignAlert> DesignAlerts { get; }
    DbSet<InsightSnapshot> InsightSnapshots { get; }
    DbSet<DesignInsightSnapshot> DesignInsightSnapshots { get; }
    DbSet<SecOpsAlert> SecOpsAlerts { get; }
    DbSet<SecOpsInsightSnapshot> SecOpsInsightSnapshots { get; }
    DbSet<SecurityScanResult> SecurityScanResults { get; }
    DbSet<MarketerAlert> MarketerAlerts { get; }
    DbSet<MarketerInsightSnapshot> MarketerInsightSnapshots { get; }
    DbSet<MarketingCampaignMetric> MarketingCampaignMetrics { get; }
    DbSet<QuickCapture> QuickCaptures { get; }
    DbSet<TeamObjective> TeamObjectives { get; }
    DbSet<TeamVaultLink> TeamVaultLinks { get; }
    DbSet<TeamArmory> TeamArmories { get; }
    DbSet<TeamMemberFocus> TeamMemberFocuses { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}