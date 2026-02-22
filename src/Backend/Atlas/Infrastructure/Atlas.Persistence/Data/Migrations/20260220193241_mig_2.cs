using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserActivities",
                newName: "UserActivities",
                newSchema: "atlas");

            migrationBuilder.RenameTable(
                name: "Snippets",
                newName: "Snippets",
                newSchema: "atlas");

            migrationBuilder.RenameTable(
                name: "FocusSessions",
                newName: "FocusSessions",
                newSchema: "atlas");

            migrationBuilder.AddColumn<bool>(
                name: "IsShared",
                schema: "atlas",
                table: "Workspaces",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LocalFolderPath",
                schema: "atlas",
                table: "Workspaces",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                schema: "identity",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Developer");

            // Safe conversion from text to jsonb for MetaData column — handle invalid JSON gracefully
            migrationBuilder.Sql(@"
DO $do$
BEGIN
    -- Create helper to attempt converting text to jsonb and return NULL on failure
    IF NOT EXISTS (SELECT 1 FROM pg_proc WHERE proname = 'try_to_jsonb') THEN
        CREATE OR REPLACE FUNCTION public.try_to_jsonb(text) RETURNS jsonb AS $fn$
        BEGIN
            RETURN $1::jsonb;
        EXCEPTION WHEN others THEN
            RETURN NULL;
        END;
        $fn$ LANGUAGE plpgsql;
    END IF;

    -- Add temporary jsonb column
    ALTER TABLE atlas.""UserActivities"" ADD COLUMN IF NOT EXISTS ""MetaData_tmp"" jsonb;

    -- Populate temporary column using the safe converter
    UPDATE atlas.""UserActivities""
    SET ""MetaData_tmp"" = try_to_jsonb(""MetaData"")
    WHERE ""MetaData"" IS NOT NULL;

    -- Drop the old column and rename the temp column
    ALTER TABLE atlas.""UserActivities"" DROP COLUMN IF EXISTS ""MetaData"";
    ALTER TABLE atlas.""UserActivities"" RENAME COLUMN ""MetaData_tmp"" TO ""MetaData"";

    -- Optional: clean up helper function
    -- DROP FUNCTION IF EXISTS public.try_to_jsonb(text);
END $do$;
");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "atlas",
                table: "UserActivities",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "atlas",
                table: "UserActivities",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                schema: "atlas",
                table: "UserActivities",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "FocusSessionId",
                schema: "atlas",
                table: "UserActivities",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                schema: "atlas",
                table: "Snippets",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                schema: "atlas",
                table: "Snippets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                schema: "atlas",
                table: "Snippets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "text",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFavorite",
                schema: "atlas",
                table: "Snippets",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "atlas",
                table: "Snippets",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                schema: "atlas",
                table: "FocusSessions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Work",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "atlas",
                table: "FocusSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                schema: "atlas",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<int>(
                name: "BreakDurationMinutes",
                schema: "atlas",
                table: "FocusSessions",
                type: "integer",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.AddColumn<DateTime>(
                name: "PausedAt",
                schema: "atlas",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionType",
                schema: "atlas",
                table: "FocusSessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                schema: "atlas",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "atlas",
                table: "FocusSessions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "WorkspaceId",
                schema: "atlas",
                table: "FocusSessions",
                type: "uuid",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"),
                column: "TargetProfession",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"),
                column: "TargetProfession",
                value: 1);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666601"),
                column: "TargetProfession",
                value: 3);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "TargetProfession",
                value: 4);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888801"),
                column: "TargetProfession",
                value: 5);

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
                name: "IX_ModalStates_UserId_HasBeenSeen",
                schema: "atlas",
                table: "ModalStates",
                columns: new[] { "UserId", "HasBeenSeen" },
                filter: "\"HasBeenSeen\" = false");

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
                name: "IX_TeamMembers_TeamId_UserId",
                schema: "atlas",
                table: "TeamMembers",
                columns: new[] { "TeamId", "UserId" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_OwnerUserId",
                schema: "atlas",
                table: "Teams",
                column: "OwnerUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotkeyBindings",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "ModalStates",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "TeamMembers",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Teams",
                schema: "atlas");

            migrationBuilder.DropIndex(
                name: "IX_UserActivities_UserId",
                schema: "atlas",
                table: "UserActivities");

            migrationBuilder.DropIndex(
                name: "IX_UserActivities_UserId_CreatedAt",
                schema: "atlas",
                table: "UserActivities");

            migrationBuilder.DropIndex(
                name: "IX_Snippets_UserId",
                schema: "atlas",
                table: "Snippets");

            migrationBuilder.DropIndex(
                name: "IX_Snippets_UserId_IsFavorite",
                schema: "atlas",
                table: "Snippets");

            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId_CompletedAt",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropIndex(
                name: "IX_FocusSessions_UserId_Status",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "IsShared",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "LocalFolderPath",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "Role",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FocusSessionId",
                schema: "atlas",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "BreakDurationMinutes",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "PausedAt",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "SessionType",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.DropColumn(
                name: "WorkspaceId",
                schema: "atlas",
                table: "FocusSessions");

            migrationBuilder.RenameTable(
                name: "UserActivities",
                schema: "atlas",
                newName: "UserActivities");

            migrationBuilder.RenameTable(
                name: "Snippets",
                schema: "atlas",
                newName: "Snippets");

            migrationBuilder.RenameTable(
                name: "FocusSessions",
                schema: "atlas",
                newName: "FocusSessions");

            migrationBuilder.AlterColumn<string>(
                name: "MetaData",
                table: "UserActivities",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "UserActivities",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "UserActivities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "ActionType",
                table: "UserActivities",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Snippets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "Snippets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Language",
                table: "Snippets",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFavorite",
                table: "Snippets",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Snippets",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Tag",
                table: "FocusSessions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Work");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "FocusSessions",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CompletedAt",
                table: "FocusSessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444401"),
                column: "TargetProfession",
                value: 3);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555501"),
                column: "TargetProfession",
                value: 4);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666601"),
                column: "TargetProfession",
                value: 5);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777701"),
                column: "TargetProfession",
                value: 6);

            migrationBuilder.UpdateData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888801"),
                column: "TargetProfession",
                value: 7);
        }
    }
}
