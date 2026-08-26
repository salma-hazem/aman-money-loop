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
            // Step 1:
            // Add UserId as nullable first so existing rows do not receive Guid.Empty.
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "OnboardingCases",
                type: "uniqueidentifier",
                nullable: true);

            // Step 2:
            // Populate UserId for existing onboarding cases using:
            // OnboardingCase -> MembershipAgreement
            // -> MembershipApplication -> UserId
            migrationBuilder.Sql(@"
                UPDATE oc
                SET oc.UserId = app.UserId
                FROM OnboardingCases oc
                INNER JOIN MembershipAgreements ma
                    ON oc.MembershipAgreementId = ma.MembershipAgreementId
                INNER JOIN MembershipApplications app
                    ON ma.MembershipApplicationId = app.MembershipApplicationId
                WHERE app.UserId IS NOT NULL;
            ");

            // Step 3:
            // Now make UserId required after existing records have been populated.
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OnboardingCases",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            // Step 4:
            // Create index.
            migrationBuilder.CreateIndex(
                name: "IX_OnboardingCases_UserId",
                table: "OnboardingCases",
                column: "UserId");

            // Step 5:
            // Create relationship with Users.
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