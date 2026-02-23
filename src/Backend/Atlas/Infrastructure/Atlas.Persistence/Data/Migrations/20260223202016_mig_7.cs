using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_BountyBoards_TeamId",
                schema: "atlas",
                table: "BountyBoards",
                column: "TeamId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BountyBoards",
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
                name: "OmniFeedItems",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SharedResources",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SquadArenaEntries",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SquadRadarEntries",
                schema: "atlas");
        }
    }
}
