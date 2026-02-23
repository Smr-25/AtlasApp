using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarketerAlerts",
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
                    table.PrimaryKey("PK_MarketerAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketerInsightSnapshots",
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
                    table.PrimaryKey("PK_MarketerInsightSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarketingCampaignMetrics",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampaignId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Spend = table.Column<double>(type: "double precision", nullable: false),
                    Revenue = table.Column<double>(type: "double precision", nullable: false),
                    Impressions = table.Column<int>(type: "integer", nullable: false),
                    Clicks = table.Column<int>(type: "integer", nullable: false),
                    Conversions = table.Column<int>(type: "integer", nullable: false),
                    MetadataJson = table.Column<string>(type: "text", nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketingCampaignMetrics", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_MarketerAlerts_UserId",
                schema: "atlas",
                table: "MarketerAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketerInsightSnapshots_UserId",
                schema: "atlas",
                table: "MarketerInsightSnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketerInsightSnapshots_UserId_Type_RecordedAt",
                schema: "atlas",
                table: "MarketerInsightSnapshots",
                columns: new[] { "UserId", "Type", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignMetrics_UserId",
                schema: "atlas",
                table: "MarketingCampaignMetrics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketingCampaignMetrics_UserId_RecordedAt",
                schema: "atlas",
                table: "MarketingCampaignMetrics",
                columns: new[] { "UserId", "RecordedAt" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketerAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "MarketerInsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "MarketingCampaignMetrics",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SecOpsAlerts",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SecOpsInsightSnapshots",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "SecurityScanResults",
                schema: "atlas");
        }
    }
}
