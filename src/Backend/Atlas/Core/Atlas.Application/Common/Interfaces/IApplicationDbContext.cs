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
    DbSet<UserOnboardingAnswer> UserOnboardingAnswers { get; }
    DbSet<UserActivity> UserActivities { get; }
    DbSet<Snippet> Snippets { get; }
    DbSet<Script> Scripts { get; }
    DbSet<FocusSession> FocusSessions { get; }
    DbSet<ProjectProfile> ProjectProfiles { get; }
    DbSet<DesignAsset> DesignAssets { get; }
    DbSet<DesignPalette> DesignPalettes { get; }
    DbSet<PaletteColor> PaletteColors { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}