using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Atlas.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class mig_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, false, null, 1, null, "What is your profession?" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 2, null, "What are your main goals for using Atlas?" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, 3, null, "Which tools do you currently use in your workflow?" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "OnboardingQuestions",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

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
    }
}
