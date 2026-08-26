using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonyLoop.Infrastructure.Data.MigrationsV2
{
    /// <inheritdoc />
    public partial class checkmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "OnboardingCases",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingCases_UserId",
                table: "OnboardingCases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OnboardingCases_Users_UserId",
                table: "OnboardingCases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OnboardingCases_Users_UserId",
                table: "OnboardingCases");

            migrationBuilder.DropIndex(
                name: "IX_OnboardingCases_UserId",
                table: "OnboardingCases");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OnboardingCases");
        }
    }
}
