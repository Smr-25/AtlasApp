using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "atlas");

            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EntityName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BountyBoards",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RewardPoints = table.Column<int>(type: "integer", nullable: false),
                    ClaimedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    JiraIssueKey = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BountyBoards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DependencyWatches",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CurrentVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LatestVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsOutdated = table.Column<bool>(type: "boolean", nullable: false),
                    HasVulnerability = table.Column<bool>(type: "boolean", nullable: false),
                    VulnerabilityDetail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ProjectPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependencyWatches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignAlerts",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActionPayload = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsActioned = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignAssets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    TargetFormat = table.Column<string>(type: "text", nullable: false),
                    OriginalSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    ConvertedSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: false),
                    IsOptimized = table.Column<bool>(type: "boolean", nullable: false),
                    OptimizedSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignHandoffs",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FigmaFileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ZeplinScreenUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DesignerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeveloperId = table.Column<Guid>(type: "uuid", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignHandoffs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignInsightSnapshots",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignInsightSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DesignPalettes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DesignPalettes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FigmaComments",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CommentId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AuthorName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FigmaComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FocusSessions",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    BreakDurationMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 5),
                    Tag = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Work"),
                    SessionType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PausedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FocusSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotkeyBindings",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    KeyCombination = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsGlobal = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotkeyBindings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsightSnapshots",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsightSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderAlerts",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActionPayload = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsActioned = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderInsightSnapshots",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderInsightSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeaderModalStates",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: true),
                    ModalType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    HasBeenSeen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DismissedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeaderModalStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModalStates",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModalType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    HasBeenSeen = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DismissedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModalStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Priority = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ActionPayloadJson = table.Column<string>(type: "jsonb", nullable: true),
                    SourceEntity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SourceEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OmniFeedItems",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OmniFeedItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    OptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomValue = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingAnswers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsMultiSelect = table.Column<bool>(type: "boolean", nullable: false),
                    TargetProfession = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutgoingWebhooks",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Secret = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Events = table.Column<string>(type: "jsonb", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConsecutiveFailures = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    LastDeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutgoingWebhooks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalAccessTokens",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TokenPrefix = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Scopes = table.Column<string>(type: "jsonb", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalAccessTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProactiveAlerts",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActionPayload = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsActioned = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProactiveAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RootPath = table.Column<string>(type: "text", nullable: false),
                    StartupProjectPath = table.Column<string>(type: "text", nullable: true),
                    MigrationProjectPath = table.Column<string>(type: "text", nullable: true),
                    LastMigrationName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuickCaptures",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Source = table.Column<int>(type: "integer", nullable: false),
                    IsSynced = table.Column<bool>(type: "boolean", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuickCaptures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scripts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Command = table.Column<string>(type: "text", nullable: false),
                    Arguments = table.Column<string>(type: "text", nullable: false),
                    WorkingDirectory = table.Column<string>(type: "text", nullable: true),
                    Icon = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: true),
                    ScriptType = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scripts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecOpsAlerts",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ActionPayload = table.Column<string>(type: "text", nullable: true),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    IsActioned = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecOpsAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecOpsInsightSnapshots",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MetricKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecOpsInsightSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityScanResults",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScanType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ResultJson = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<double>(type: "double precision", nullable: true),
                    ScannedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityScanResults", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SentryIssues",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Culprit = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    LineNumber = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    EventCount = table.Column<int>(type: "integer", nullable: false),
                    IsResolved = table.Column<bool>(type: "boolean", nullable: false),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentryIssues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SharedResources",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    UploadedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPinned = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snippets",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "text"),
                    Tags = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snippets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SonarQubeReports",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReliabilityGrade = table.Column<int>(type: "integer", nullable: false),
                    SecurityGrade = table.Column<int>(type: "integer", nullable: false),
                    MaintainabilityGrade = table.Column<int>(type: "integer", nullable: false),
                    CoveragePercent = table.Column<double>(type: "double precision", nullable: false),
                    TotalIssues = table.Column<int>(type: "integer", nullable: false),
                    Bugs = table.Column<int>(type: "integer", nullable: false),
                    Vulnerabilities = table.Column<int>(type: "integer", nullable: false),
                    CodeSmells = table.Column<int>(type: "integer", nullable: false),
                    DuplicatedLines = table.Column<int>(type: "integer", nullable: false),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SonarQubeReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SquadArenaEntries",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    BadgeType = table.Column<int>(type: "integer", nullable: false),
                    SprintId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    AwardedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadArenaEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SquadRadarEntries",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OnlineStatus = table.Column<int>(type: "integer", nullable: false),
                    CurrentToolIcon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CurrentFocus = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LastActiveAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActiveIntegrationsJson = table.Column<string>(type: "jsonb", nullable: true),
                    MeetingMinutesLeft = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SquadRadarEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tier = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    StripeCustomerId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StripeSubscriptionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxWorkspaces = table.Column<int>(type: "integer", nullable: false),
                    MaxIntegrations = table.Column<int>(type: "integer", nullable: false),
                    HasCustomHotkeys = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Subject = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Body = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    PageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BrowserInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AdminReply = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    RepliedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamArmories",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    StagingServerUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    IsStagingOnline = table.Column<bool>(type: "boolean", nullable: false),
                    TestAccountEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    TestAccountPassword = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProductionVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StagingVersion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamArmories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberFocuses",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    FocusDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberFocuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamObjectives",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamObjectives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxMembers = table.Column<int>(type: "integer", nullable: false, defaultValue: 7),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeamVaultLinks",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Icon = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamVaultLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: true),
                    FocusSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MetaData = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    Theme = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "system"),
                    Timezone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "UTC"),
                    EmailNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    PushNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    InboxAlerts = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    InboxApprovals = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    InboxMentions = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    InboxSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    WeeklyDigest = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CustomSettingsJson = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Profession = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThemeColor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "#007AFF"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Developer"),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailVerificationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EmailVerificationExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PreferredVerificationChannel = table.Column<int>(type: "integer", nullable: true),
                    TelegramChatId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TelegramLinkCode = table.Column<string>(type: "text", nullable: true),
                    TelegramLinkCodeExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhoneVerificationCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PhoneVerificationExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RefreshToken = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ResetPasswordCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ResetPasswordExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    LockoutEndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaletteColors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    HexCode = table.Column<string>(type: "text", nullable: false),
                    PaletteId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignPaletteId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaletteColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaletteColors_DesignPalettes_DesignPaletteId",
                        column: x => x.DesignPaletteId,
                        principalTable: "DesignPalettes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OnboardingOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    RecommendedIntegration = table.Column<string>(type: "text", nullable: true),
                    RecommendedTemplate = table.Column<string>(type: "text", nullable: true),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnboardingOptions_OnboardingQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "OnboardingQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamMembers",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TeamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamMembers_Teams_TeamId",
                        column: x => x.TeamId,
                        principalSchema: "atlas",
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Integrations",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Scope = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Personal"),
                    ApiUrl = table.Column<string>(type: "text", nullable: true),
                    EncryptedAccessToken = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EncryptedRefreshToken = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TokenExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MetadataJson = table.Column<string>(type: "jsonb", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Integrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Integrations_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "atlas",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workspaces",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LocalFolderPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsShared = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workspaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workspaces_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalSchema: "atlas",
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceIntegrations",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SettingsJson = table.Column<string>(type: "jsonb", nullable: true),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceIntegrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceIntegrations_Integrations_IntegrationId",
                        column: x => x.IntegrationId,
                        principalSchema: "atlas",
                        principalTable: "Integrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkspaceIntegrations_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "atlas",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkspaceMembers",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkspaceMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkspaceMembers_Workspaces_WorkspaceId",
                        column: x => x.WorkspaceId,
                        principalSchema: "atlas",
                        principalTable: "Workspaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                schema: "atlas",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                schema: "atlas",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId_Action",
                schema: "atlas",
                table: "AuditLogs",
                columns: new[] { "UserId", "Action" });

            migrationBuilder.CreateIndex(
                name: "IX_BountyBoards_TeamId",
                schema: "atlas",
                table: "BountyBoards",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_DependencyWatches_UserId",
                schema: "atlas",
                table: "DependencyWatches",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignAlerts_UserId",
                schema: "atlas",
                table: "DesignAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignHandoffs_DesignerId",
                schema: "atlas",
                table: "DesignHandoffs",
                column: "DesignerId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignHandoffs_DeveloperId",
                schema: "atlas",
                table: "DesignHandoffs",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignInsightSnapshots_UserId",
                schema: "atlas",
                table: "DesignInsightSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DesignInsightSnapshots_UserId_Type_RecordedAt",
                schema: "atlas",
                table: "DesignInsightSnapshots",
                columns: new[] { "UserId", "Type", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FigmaComments_IntegrationId",
                schema: "atlas",
                table: "FigmaComments",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_FigmaComments_UserId",
                schema: "atlas",
                table: "FigmaComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId",
                schema: "atlas",
                table: "FocusSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId_CompletedAt",
                schema: "atlas",
                table: "FocusSessions",
                columns: new[] { "UserId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FocusSessions_UserId_Status",
                schema: "atlas",
                table: "FocusSessions",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_HotkeyBindings_UserId",
                schema: "atlas",
                table: "HotkeyBindings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HotkeyBindings_UserId_Action",
                schema: "atlas",
                table: "HotkeyBindings",
                columns: new[] { "UserId", "Action" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_InsightSnapshots_UserId",
                schema: "atlas",
                table: "InsightSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InsightSnapshots_UserId_Type_RecordedAt",
                schema: "atlas",
                table: "InsightSnapshots",
                columns: new[] { "UserId", "Type", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_Status",
                schema: "atlas",
                table: "Integrations",
                column: "Status",
                filter: "\"Status\" = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_UserProfileId",
                schema: "atlas",
                table: "Integrations",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_UserProfileId_Provider",
                schema: "atlas",
                table: "Integrations",
                columns: new[] { "UserProfileId", "Provider" });

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_UserProfileId_Provider_Name",
                schema: "atlas",
                table: "Integrations",
                columns: new[] { "UserProfileId", "Provider", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderAlerts_TeamId",
                schema: "atlas",
                table: "LeaderAlerts",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderAlerts_UserId",
                schema: "atlas",
                table: "LeaderAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderInsightSnapshots_UserId",
                schema: "atlas",
                table: "LeaderInsightSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LeaderInsightSnapshots_UserId_Type_RecordedAt",
                schema: "atlas",
                table: "LeaderInsightSnapshots",
                columns: new[] { "UserId", "Type", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LeaderModalStates_UserId_ModalType",
                schema: "atlas",
                table: "LeaderModalStates",
                columns: new[] { "UserId", "ModalType" });

            migrationBuilder.CreateIndex(
                name: "IX_ModalStates_UserId_HasBeenSeen",
                schema: "atlas",
                table: "ModalStates",
                columns: new[] { "UserId", "HasBeenSeen" },
                filter: "\"HasBeenSeen\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "atlas",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Category",
                schema: "atlas",
                table: "Notifications",
                columns: new[] { "UserId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                schema: "atlas",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_OmniFeedItems_Source",
                schema: "atlas",
                table: "OmniFeedItems",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_OmniFeedItems_TeamId_Timestamp",
                schema: "atlas",
                table: "OmniFeedItems",
                columns: new[] { "TeamId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingOptions_QuestionId",
                table: "OnboardingOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OutgoingWebhooks_UserId",
                schema: "atlas",
                table: "OutgoingWebhooks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaletteColors_DesignPaletteId",
                table: "PaletteColors",
                column: "DesignPaletteId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalAccessTokens_TokenHash",
                schema: "atlas",
                table: "PersonalAccessTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalAccessTokens_UserId",
                schema: "atlas",
                table: "PersonalAccessTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProactiveAlerts_UserId",
                schema: "atlas",
                table: "ProactiveAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProactiveAlerts_UserId_IsRead",
                schema: "atlas",
                table: "ProactiveAlerts",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_QuickCaptures_UserId",
                schema: "atlas",
                table: "QuickCaptures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuickCaptures_UserId_Source",
                schema: "atlas",
                table: "QuickCaptures",
                columns: new[] { "UserId", "Source" });

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SecOpsAlerts_UserId",
                schema: "atlas",
                table: "SecOpsAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecOpsInsightSnapshots_UserId",
                schema: "atlas",
                table: "SecOpsInsightSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SecOpsInsightSnapshots_UserId_Type_RecordedAt",
                schema: "atlas",
                table: "SecOpsInsightSnapshots",
                columns: new[] { "UserId", "Type", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SecurityScanResults_UserId",
                schema: "atlas",
                table: "SecurityScanResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SentryIssues_IntegrationId",
                schema: "atlas",
                table: "SentryIssues",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_SentryIssues_UserId",
                schema: "atlas",
                table: "SentryIssues",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedResources_TeamId",
                schema: "atlas",
                table: "SharedResources",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedResources_TeamId_Category",
                schema: "atlas",
                table: "SharedResources",
                columns: new[] { "TeamId", "Category" });

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_UserId",
                schema: "atlas",
                table: "Snippets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Snippets_UserId_IsFavorite",
                schema: "atlas",
                table: "Snippets",
                columns: new[] { "UserId", "IsFavorite" },
                filter: "\"IsFavorite\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_SonarQubeReports_IntegrationId",
                schema: "atlas",
                table: "SonarQubeReports",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_SonarQubeReports_UserId",
                schema: "atlas",
                table: "SonarQubeReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SquadArenaEntries_TeamId_BadgeType",
                schema: "atlas",
                table: "SquadArenaEntries",
                columns: new[] { "TeamId", "BadgeType" });

            migrationBuilder.CreateIndex(
                name: "IX_SquadArenaEntries_TeamId_UserId",
                schema: "atlas",
                table: "SquadArenaEntries",
                columns: new[] { "TeamId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_SquadRadarEntries_TeamId_UserId",
                schema: "atlas",
                table: "SquadRadarEntries",
                columns: new[] { "TeamId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_StripeCustomerId",
                schema: "atlas",
                table: "Subscriptions",
                column: "StripeCustomerId",
                filter: "\"StripeCustomerId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                schema: "atlas",
                table: "Subscriptions",
                column: "UserId",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_Status",
                schema: "atlas",
                table: "SupportTickets",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_UserId",
                schema: "atlas",
                table: "SupportTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamArmories_TeamId",
                schema: "atlas",
                table: "TeamArmories",
                column: "TeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberFocuses_TeamId",
                schema: "atlas",
                table: "TeamMemberFocuses",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberFocuses_TeamMemberId_IsActive",
                schema: "atlas",
                table: "TeamMemberFocuses",
                columns: new[] { "TeamMemberId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamMembers_TeamId_UserId",
                schema: "atlas",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_TeamObjectives_TeamId",
                schema: "atlas",
                table: "TeamObjectives",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamObjectives_TeamId_IsActive",
                schema: "atlas",
                table: "TeamObjectives",
                columns: new[] { "TeamId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Teams_OwnerUserId",
                schema: "atlas",
                table: "Teams",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamVaultLinks_TeamId",
                schema: "atlas",
                table: "TeamVaultLinks",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                schema: "atlas",
                table: "UserActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId_CreatedAt",
                schema: "atlas",
                table: "UserActivities",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferences_UserId",
                schema: "atlas",
                table: "UserPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Profession",
                schema: "atlas",
                table: "UserProfiles",
                column: "Profession");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "identity",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                schema: "identity",
                table: "Users",
                column: "PhoneNumber",
                unique: true,
                filter: "\"PhoneNumber\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "identity",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceIntegrations_IntegrationId",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceIntegrations_WorkspaceId",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceIntegrations_WorkspaceId_IntegrationId",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                columns: new[] { "WorkspaceId", "IntegrationId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_UserId",
                schema: "atlas",
                table: "WorkspaceMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_WorkspaceId",
                schema: "atlas",
                table: "WorkspaceMembers",
                column: "WorkspaceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkspaceMembers_WorkspaceId_UserId",
                schema: "atlas",
                table: "WorkspaceMembers",
                columns: new[] { "WorkspaceId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserProfileId",
                schema: "atlas",
                table: "Workspaces",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserProfileId_IsDefault",
                schema: "atlas",
                table: "Workspaces",
                columns: new[] { "UserProfileId", "IsDefault" },
                filter: "\"IsDefault\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_UserProfileId_Name",
                schema: "atlas",
                table: "Workspaces",
                columns: new[] { "UserProfileId", "Name" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "BountyBoards",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DependencyWatches",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DesignAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DesignAssets");

            migrationBuilder.DropTable(
                name: "DesignHandoffs",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DesignInsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "FigmaComments",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "FocusSessions",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "HotkeyBindings",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "InsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "LeaderAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "LeaderInsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "LeaderModalStates",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "ModalStates",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "OmniFeedItems",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "OnboardingAnswers");

            migrationBuilder.DropTable(
                name: "OnboardingOptions");

            migrationBuilder.DropTable(
                name: "OutgoingWebhooks",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "PaletteColors");

            migrationBuilder.DropTable(
                name: "PersonalAccessTokens",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "ProactiveAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "ProjectProfiles");

            migrationBuilder.DropTable(
                name: "QuickCaptures",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Scripts");

            migrationBuilder.DropTable(
                name: "SecOpsAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SecOpsInsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SecurityScanResults",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SentryIssues",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SharedResources",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Snippets",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SonarQubeReports",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SquadArenaEntries",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SquadRadarEntries",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SupportTickets",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamArmories",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamMemberFocuses",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamMembers",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamObjectives",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamVaultLinks",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "UserActivities",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "WorkspaceIntegrations",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "WorkspaceMembers",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "OnboardingQuestions");

            migrationBuilder.DropTable(
                name: "DesignPalettes");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "Integrations",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Workspaces",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "atlas");
        }
    }
}
