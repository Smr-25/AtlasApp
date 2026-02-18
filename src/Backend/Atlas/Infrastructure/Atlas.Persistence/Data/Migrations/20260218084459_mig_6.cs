using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_Personas_PersonaId",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_OnboardingQuestions_Interests_InterestId",
                table: "OnboardingQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_OnboardingQuestions_Professions_ProfessionId",
                table: "OnboardingQuestions");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Personas_PersonaId",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Personas",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropIndex(
                name: "IX_Workspaces_LastAccessedAt",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropIndex(
                name: "IX_OnboardingQuestions_InterestId",
                table: "OnboardingQuestions");

            migrationBuilder.DropIndex(
                name: "IX_OnboardingQuestions_ProfessionId",
                table: "OnboardingQuestions");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_IsActive",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "Color",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "Config",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropColumn(
                name: "InterestId",
                table: "OnboardingQuestions");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "OnboardingQuestions");

            migrationBuilder.DropColumn(
                name: "BioPart",
                table: "OnboardingOptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "LastUsedAt",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                schema: "atlas",
                table: "Workspaces",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_PersonaId_Name",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_UserProfileId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_PersonaId_IsDefault",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_UserProfileId_IsDefault");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_PersonaId",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_UserProfileId");

            migrationBuilder.RenameColumn(
                name: "Config",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                newName: "SettingsJson");

            migrationBuilder.RenameColumn(
                name: "RefreshToken",
                schema: "atlas",
                table: "Integrations",
                newName: "EncryptedRefreshToken");

            migrationBuilder.RenameColumn(
                name: "PersonaId",
                schema: "atlas",
                table: "Integrations",
                newName: "UserProfileId");

            migrationBuilder.RenameColumn(
                name: "Metadata",
                schema: "atlas",
                table: "Integrations",
                newName: "MetadataJson");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_PersonaId_Provider_Name",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_UserProfileId_Provider_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_PersonaId_Provider",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_UserProfileId_Provider");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_PersonaId",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_UserProfileId");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "OnboardingQuestions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "TargetProfession",
                table: "OnboardingQuestions",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "OnboardingOptions",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "RecommendedIntegration",
                table: "OnboardingOptions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecommendedTemplate",
                table: "OnboardingOptions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedAccessToken",
                schema: "atlas",
                table: "Integrations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApiUrl",
                schema: "atlas",
                table: "Integrations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "atlas",
                table: "Integrations",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.InsertData(
                table: "OnboardingQuestions",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "IsMultiSelect", "ModifiedAt", "Order", "TargetProfession", "Text" },
                values: new object[,]
                {
                    { new Guid("3ff43c35-f411-408e-b46d-021e69e360f2"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3630), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 3, null, "Which tools do you currently use in your workflow?" },
                    { new Guid("64b9c897-e9a2-404e-9225-6ea443c7574f"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3140), new TimeSpan(0, 0, 0, 0, 0)), false, false, null, 1, null, "What is your profession?" },
                    { new Guid("e86212eb-e481-41d6-8b05-7bf9dd4a1cd6"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3630), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 2, null, "What are your main goals for using Atlas?" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_Status",
                schema: "atlas",
                table: "Integrations",
                column: "Status",
                filter: "\"Status\" = 'Active'");

            migrationBuilder.CreateIndex(
                name: "IX_PaletteColors_DesignPaletteId",
                table: "PaletteColors",
                column: "DesignPaletteId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Profession",
                schema: "atlas",
                table: "UserProfiles",
                column: "Profession");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_UserProfiles_UserProfileId",
                schema: "atlas",
                table: "Integrations",
                column: "UserProfileId",
                principalSchema: "atlas",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_UserProfiles_UserProfileId",
                schema: "atlas",
                table: "Workspaces",
                column: "UserProfileId",
                principalSchema: "atlas",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrations_UserProfiles_UserProfileId",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_UserProfiles_UserProfileId",
                schema: "atlas",
                table: "Workspaces");

            migrationBuilder.DropTable(
                name: "DesignAssets");

            migrationBuilder.DropTable(
                name: "OnboardingAnswers");

            migrationBuilder.DropTable(
                name: "PaletteColors");

            migrationBuilder.DropTable(
                name: "UserProfiles",
                schema: "atlas");

            migrationBuilder.DropTable(
                name: "DesignPalettes");

            migrationBuilder.DropIndex(
                name: "IX_Integrations_Status",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("3ff43c35-f411-408e-b46d-021e69e360f2"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("64b9c897-e9a2-404e-9225-6ea443c7574f"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("e86212eb-e481-41d6-8b05-7bf9dd4a1cd6"));

            migrationBuilder.DropColumn(
                name: "Enabled",
                schema: "atlas",
                table: "WorkspaceIntegrations");

            migrationBuilder.DropColumn(
                name: "TargetProfession",
                table: "OnboardingQuestions");

            migrationBuilder.DropColumn(
                name: "RecommendedIntegration",
                table: "OnboardingOptions");

            migrationBuilder.DropColumn(
                name: "RecommendedTemplate",
                table: "OnboardingOptions");

            migrationBuilder.DropColumn(
                name: "ApiUrl",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "atlas",
                table: "Integrations");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                schema: "atlas",
                table: "Workspaces",
                newName: "PersonaId");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_UserProfileId_Name",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_PersonaId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_UserProfileId_IsDefault",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_PersonaId_IsDefault");

            migrationBuilder.RenameIndex(
                name: "IX_Workspaces_UserProfileId",
                schema: "atlas",
                table: "Workspaces",
                newName: "IX_Workspaces_PersonaId");

            migrationBuilder.RenameColumn(
                name: "SettingsJson",
                schema: "atlas",
                table: "WorkspaceIntegrations",
                newName: "Config");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                schema: "atlas",
                table: "Integrations",
                newName: "PersonaId");

            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                schema: "atlas",
                table: "Integrations",
                newName: "Metadata");

            migrationBuilder.RenameColumn(
                name: "EncryptedRefreshToken",
                schema: "atlas",
                table: "Integrations",
                newName: "RefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_UserProfileId_Provider_Name",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_PersonaId_Provider_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_UserProfileId_Provider",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_PersonaId_Provider");

            migrationBuilder.RenameIndex(
                name: "IX_Integrations_UserProfileId",
                schema: "atlas",
                table: "Integrations",
                newName: "IX_Integrations_PersonaId");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "atlas",
                table: "Workspaces",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Config",
                schema: "atlas",
                table: "Workspaces",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "atlas",
                table: "Workspaces",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastAccessedAt",
                schema: "atlas",
                table: "Workspaces",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "OnboardingQuestions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<Guid>(
                name: "InterestId",
                table: "OnboardingQuestions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "OnboardingQuestions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "OnboardingOptions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "BioPart",
                table: "OnboardingOptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "EncryptedAccessToken",
                schema: "atlas",
                table: "Integrations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "atlas",
                table: "Integrations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUsedAt",
                schema: "atlas",
                table: "Integrations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                schema: "atlas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Bio = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Config = table.Column<string>(type: "jsonb", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personas_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workspaces_LastAccessedAt",
                schema: "atlas",
                table: "Workspaces",
                column: "LastAccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingQuestions_InterestId",
                table: "OnboardingQuestions",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingQuestions_ProfessionId",
                table: "OnboardingQuestions",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Integrations_IsActive",
                schema: "atlas",
                table: "Integrations",
                column: "IsActive",
                filter: "\"IsActive\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_UserId",
                schema: "atlas",
                table: "Personas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_UserId_IsPrimary",
                schema: "atlas",
                table: "Personas",
                columns: new[] { "UserId", "IsPrimary" },
                filter: "\"IsPrimary\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_UserId_Type",
                schema: "atlas",
                table: "Personas",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.AddForeignKey(
                name: "FK_Integrations_Personas_PersonaId",
                schema: "atlas",
                table: "Integrations",
                column: "PersonaId",
                principalSchema: "atlas",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OnboardingQuestions_Interests_InterestId",
                table: "OnboardingQuestions",
                column: "InterestId",
                principalTable: "Interests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OnboardingQuestions_Professions_ProfessionId",
                table: "OnboardingQuestions",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Personas_PersonaId",
                schema: "atlas",
                table: "Workspaces",
                column: "PersonaId",
                principalSchema: "atlas",
                principalTable: "Personas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
