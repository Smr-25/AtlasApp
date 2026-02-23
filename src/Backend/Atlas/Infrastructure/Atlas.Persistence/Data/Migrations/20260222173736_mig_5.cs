using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScriptType",
                table: "Scripts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptimized",
                table: "DesignAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "OptimizedSizeBytes",
                table: "DesignAssets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "AwsDeployments",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Environment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CommitSha = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    LogUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IntegrationId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AwsDeployments", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_AwsDeployments_IntegrationId",
                schema: "atlas",
                table: "AwsDeployments",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_AwsDeployments_UserId",
                schema: "atlas",
                table: "AwsDeployments",
                column: "UserId");

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
                name: "IX_SonarQubeReports_IntegrationId",
                schema: "atlas",
                table: "SonarQubeReports",
                column: "IntegrationId");

            migrationBuilder.CreateIndex(
                name: "IX_SonarQubeReports_UserId",
                schema: "atlas",
                table: "SonarQubeReports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AwsDeployments",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DependencyWatches",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DesignAlerts",
                schema: "atlas");

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
                name: "InsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "ProactiveAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SentryIssues",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SonarQubeReports",
                schema: "atlas");

            migrationBuilder.DropColumn(
                name: "ScriptType",
                table: "Scripts");

            migrationBuilder.DropColumn(
                name: "IsOptimized",
                table: "DesignAssets");

            migrationBuilder.DropColumn(
                name: "OptimizedSizeBytes",
                table: "DesignAssets");
        }
    }
}
