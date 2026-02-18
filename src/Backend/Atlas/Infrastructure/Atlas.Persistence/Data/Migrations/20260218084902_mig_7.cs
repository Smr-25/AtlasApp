using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "OnboardingQuestions",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "IsMultiSelect", "ModifiedAt", "Order", "TargetProfession", "Text" },
                values: new object[,]
                {
                    { new Guid("2c8389ed-c9b1-4f51-b7d7-52e5153f579c"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 49, 2, 25, DateTimeKind.Unspecified).AddTicks(9170), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 3, null, "Which tools do you currently use in your workflow?" },
                    { new Guid("b58d3a2f-f308-4082-8a85-9a8060052d01"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 49, 2, 25, DateTimeKind.Unspecified).AddTicks(8710), new TimeSpan(0, 0, 0, 0, 0)), false, false, null, 1, null, "What is your profession?" },
                    { new Guid("e3fe71ae-10dc-437e-93d6-a0d32563de24"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 49, 2, 25, DateTimeKind.Unspecified).AddTicks(9170), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 2, null, "What are your main goals for using Atlas?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("2c8389ed-c9b1-4f51-b7d7-52e5153f579c"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("b58d3a2f-f308-4082-8a85-9a8060052d01"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("e3fe71ae-10dc-437e-93d6-a0d32563de24"));

            migrationBuilder.InsertData(
                table: "OnboardingQuestions",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "IsMultiSelect", "ModifiedAt", "Order", "TargetProfession", "Text" },
                values: new object[,]
                {
                    { new Guid("3ff43c35-f411-408e-b46d-021e69e360f2"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3630), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 3, null, "Which tools do you currently use in your workflow?" },
                    { new Guid("64b9c897-e9a2-404e-9225-6ea443c7574f"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3140), new TimeSpan(0, 0, 0, 0, 0)), false, false, null, 1, null, "What is your profession?" },
                    { new Guid("e86212eb-e481-41d6-8b05-7bf9dd4a1cd6"), new DateTimeOffset(new DateTime(2026, 2, 18, 8, 44, 58, 963, DateTimeKind.Unspecified).AddTicks(3630), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 2, null, "What are your main goals for using Atlas?" }
                });
        }
    }
}
