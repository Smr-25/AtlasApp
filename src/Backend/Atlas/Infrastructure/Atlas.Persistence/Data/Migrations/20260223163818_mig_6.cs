using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_TeamVaultLinks_TeamId",
                schema: "atlas",
                table: "TeamVaultLinks",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuickCaptures",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamArmories",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamMemberFocuses",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamObjectives",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamVaultLinks",
                schema: "atlas");
        }
    }
}
