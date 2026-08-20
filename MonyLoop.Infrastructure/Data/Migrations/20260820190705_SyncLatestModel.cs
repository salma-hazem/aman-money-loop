using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonyLoop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncLatestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberLedgers_CircleSlots_CircleSlotId",
                table: "MemberLedgers");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationChecklistSubmission_VerificationSchedule_VerificationScheduleId",
                table: "VerificationChecklistSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriterion_VerificationRound_VerificationRoundId",
                table: "VerificationCriterion");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriterionRating_VerificationChecklistSubmission_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRating");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriterionRating_VerificationCriterion_VerificationCriterionId",
                table: "VerificationCriterionRating");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationRound_Circles_CircleId",
                table: "VerificationRound");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationSchedule_MembershipApplications_ApplicationId",
                table: "VerificationSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationSchedule_VerificationRound_VerificationRoundId",
                table: "VerificationSchedule");

            migrationBuilder.DropIndex(
                name: "IX_OTPTokens_UserId",
                table: "OTPTokens");

            migrationBuilder.DropIndex(
                name: "IX_MemberLedgers_CircleSlotId",
                table: "MemberLedgers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationSchedule",
                table: "VerificationSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationRound",
                table: "VerificationRound");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCriterionRating",
                table: "VerificationCriterionRating");

            migrationBuilder.DropIndex(
                name: "IX_VerificationCriterionRating_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCriterion",
                table: "VerificationCriterion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationChecklistSubmission",
                table: "VerificationChecklistSubmission");

            migrationBuilder.DropColumn(
                name: "CircleSlotId",
                table: "MemberLedgers");

            migrationBuilder.RenameTable(
                name: "VerificationSchedule",
                newName: "VerificationSchedules");

            migrationBuilder.RenameTable(
                name: "VerificationRound",
                newName: "VerificationRounds");

            migrationBuilder.RenameTable(
                name: "VerificationCriterionRating",
                newName: "VerificationCriterionRatings");

            migrationBuilder.RenameTable(
                name: "VerificationCriterion",
                newName: "VerificationCriteria");

            migrationBuilder.RenameTable(
                name: "VerificationChecklistSubmission",
                newName: "VerificationChecklistSubmissions");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationSchedule_VerificationRoundId",
                table: "VerificationSchedules",
                newName: "IX_VerificationSchedules_VerificationRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationSchedule_ApplicationId",
                table: "VerificationSchedules",
                newName: "IX_VerificationSchedules_ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationRound_CircleId",
                table: "VerificationRounds",
                newName: "IX_VerificationRounds_CircleId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCriterionRating_VerificationCriterionId",
                table: "VerificationCriterionRatings",
                newName: "IX_VerificationCriterionRatings_VerificationCriterionId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCriterion_VerificationRoundId",
                table: "VerificationCriteria",
                newName: "IX_VerificationCriteria_VerificationRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationChecklistSubmission_VerificationScheduleId",
                table: "VerificationChecklistSubmissions",
                newName: "IX_VerificationChecklistSubmissions_VerificationScheduleId");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OTPTokens",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddColumn<int>(
                name: "AttemptsCount",
                table: "OTPTokens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ResponseTokenHash",
                table: "MembershipAgreements",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationSchedules",
                table: "VerificationSchedules",
                column: "VerificationScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationRounds",
                table: "VerificationRounds",
                column: "VerificationRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCriterionRatings",
                table: "VerificationCriterionRatings",
                column: "VerificationCriterionRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCriteria",
                table: "VerificationCriteria",
                column: "VerificationCriterionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationChecklistSubmissions",
                table: "VerificationChecklistSubmissions",
                column: "VerificationChecklistSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_RecordedByUserId",
                table: "PaymentTransactions",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPTokens_UserId_Purpose",
                table: "OTPTokens",
                columns: new[] { "UserId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCriterionRatings_VerificationChecklistSubmissionId_VerificationCriterionId",
                table: "VerificationCriterionRatings",
                columns: new[] { "VerificationChecklistSubmissionId", "VerificationCriterionId" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_VerificationCriterionRating_Rating",
                table: "VerificationCriterionRatings",
                sql: "[Rating] BETWEEN 1 AND 5");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleRequests_Users_CreatedByOrganizerId",
                table: "CircleRequests",
                column: "CreatedByOrganizerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleRequests_Users_ReviewedByAdminId",
                table: "CircleRequests",
                column: "ReviewedByAdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CircleSlots_MemberLedgers_MemberLedgerId",
                table: "CircleSlots",
                column: "MemberLedgerId",
                principalTable: "MemberLedgers",
                principalColumn: "MemberLedgerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentTransactions_Users_RecordedByUserId",
                table: "PaymentTransactions",
                column: "RecordedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationChecklistSubmissions_VerificationSchedules_VerificationScheduleId",
                table: "VerificationChecklistSubmissions",
                column: "VerificationScheduleId",
                principalTable: "VerificationSchedules",
                principalColumn: "VerificationScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriteria_VerificationRounds_VerificationRoundId",
                table: "VerificationCriteria",
                column: "VerificationRoundId",
                principalTable: "VerificationRounds",
                principalColumn: "VerificationRoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriterionRatings_VerificationChecklistSubmissions_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRatings",
                column: "VerificationChecklistSubmissionId",
                principalTable: "VerificationChecklistSubmissions",
                principalColumn: "VerificationChecklistSubmissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriterionRatings_VerificationCriteria_VerificationCriterionId",
                table: "VerificationCriterionRatings",
                column: "VerificationCriterionId",
                principalTable: "VerificationCriteria",
                principalColumn: "VerificationCriterionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationRounds_Circles_CircleId",
                table: "VerificationRounds",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "CircleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationSchedules_MembershipApplications_ApplicationId",
                table: "VerificationSchedules",
                column: "ApplicationId",
                principalTable: "MembershipApplications",
                principalColumn: "MembershipApplicationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationSchedules_VerificationRounds_VerificationRoundId",
                table: "VerificationSchedules",
                column: "VerificationRoundId",
                principalTable: "VerificationRounds",
                principalColumn: "VerificationRoundId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_PerformedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleRequests_Users_CreatedByOrganizerId",
                table: "CircleRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleRequests_Users_ReviewedByAdminId",
                table: "CircleRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleSlots_MemberLedgers_MemberLedgerId",
                table: "CircleSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentTransactions_Users_RecordedByUserId",
                table: "PaymentTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationChecklistSubmissions_VerificationSchedules_VerificationScheduleId",
                table: "VerificationChecklistSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriteria_VerificationRounds_VerificationRoundId",
                table: "VerificationCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriterionRatings_VerificationChecklistSubmissions_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationCriterionRatings_VerificationCriteria_VerificationCriterionId",
                table: "VerificationCriterionRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationRounds_Circles_CircleId",
                table: "VerificationRounds");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationSchedules_MembershipApplications_ApplicationId",
                table: "VerificationSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_VerificationSchedules_VerificationRounds_VerificationRoundId",
                table: "VerificationSchedules");

            migrationBuilder.DropIndex(
                name: "IX_PaymentTransactions_RecordedByUserId",
                table: "PaymentTransactions");

            migrationBuilder.DropIndex(
                name: "IX_OTPTokens_UserId_Purpose",
                table: "OTPTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationSchedules",
                table: "VerificationSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationRounds",
                table: "VerificationRounds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCriterionRatings",
                table: "VerificationCriterionRatings");

            migrationBuilder.DropIndex(
                name: "IX_VerificationCriterionRatings_VerificationChecklistSubmissionId_VerificationCriterionId",
                table: "VerificationCriterionRatings");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VerificationCriterionRating_Rating",
                table: "VerificationCriterionRatings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationCriteria",
                table: "VerificationCriteria");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VerificationChecklistSubmissions",
                table: "VerificationChecklistSubmissions");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AttemptsCount",
                table: "OTPTokens");

            migrationBuilder.DropColumn(
                name: "ResponseTokenHash",
                table: "MembershipAgreements");

            migrationBuilder.RenameTable(
                name: "VerificationSchedules",
                newName: "VerificationSchedule");

            migrationBuilder.RenameTable(
                name: "VerificationRounds",
                newName: "VerificationRound");

            migrationBuilder.RenameTable(
                name: "VerificationCriterionRatings",
                newName: "VerificationCriterionRating");

            migrationBuilder.RenameTable(
                name: "VerificationCriteria",
                newName: "VerificationCriterion");

            migrationBuilder.RenameTable(
                name: "VerificationChecklistSubmissions",
                newName: "VerificationChecklistSubmission");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationSchedules_VerificationRoundId",
                table: "VerificationSchedule",
                newName: "IX_VerificationSchedule_VerificationRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationSchedules_ApplicationId",
                table: "VerificationSchedule",
                newName: "IX_VerificationSchedule_ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationRounds_CircleId",
                table: "VerificationRound",
                newName: "IX_VerificationRound_CircleId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCriterionRatings_VerificationCriterionId",
                table: "VerificationCriterionRating",
                newName: "IX_VerificationCriterionRating_VerificationCriterionId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationCriteria_VerificationRoundId",
                table: "VerificationCriterion",
                newName: "IX_VerificationCriterion_VerificationRoundId");

            migrationBuilder.RenameIndex(
                name: "IX_VerificationChecklistSubmissions_VerificationScheduleId",
                table: "VerificationChecklistSubmission",
                newName: "IX_VerificationChecklistSubmission_VerificationScheduleId");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "OTPTokens",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<Guid>(
                name: "CircleSlotId",
                table: "MemberLedgers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationSchedule",
                table: "VerificationSchedule",
                column: "VerificationScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationRound",
                table: "VerificationRound",
                column: "VerificationRoundId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCriterionRating",
                table: "VerificationCriterionRating",
                column: "VerificationCriterionRatingId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationCriterion",
                table: "VerificationCriterion",
                column: "VerificationCriterionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VerificationChecklistSubmission",
                table: "VerificationChecklistSubmission",
                column: "VerificationChecklistSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPTokens_UserId",
                table: "OTPTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgers_CircleSlotId",
                table: "MemberLedgers",
                column: "CircleSlotId",
                unique: true,
                filter: "[CircleSlotId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCriterionRating_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRating",
                column: "VerificationChecklistSubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberLedgers_CircleSlots_CircleSlotId",
                table: "MemberLedgers",
                column: "CircleSlotId",
                principalTable: "CircleSlots",
                principalColumn: "CircleSlotId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationChecklistSubmission_VerificationSchedule_VerificationScheduleId",
                table: "VerificationChecklistSubmission",
                column: "VerificationScheduleId",
                principalTable: "VerificationSchedule",
                principalColumn: "VerificationScheduleId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriterion_VerificationRound_VerificationRoundId",
                table: "VerificationCriterion",
                column: "VerificationRoundId",
                principalTable: "VerificationRound",
                principalColumn: "VerificationRoundId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriterionRating_VerificationChecklistSubmission_VerificationChecklistSubmissionId",
                table: "VerificationCriterionRating",
                column: "VerificationChecklistSubmissionId",
                principalTable: "VerificationChecklistSubmission",
                principalColumn: "VerificationChecklistSubmissionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationCriterionRating_VerificationCriterion_VerificationCriterionId",
                table: "VerificationCriterionRating",
                column: "VerificationCriterionId",
                principalTable: "VerificationCriterion",
                principalColumn: "VerificationCriterionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationRound_Circles_CircleId",
                table: "VerificationRound",
                column: "CircleId",
                principalTable: "Circles",
                principalColumn: "CircleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationSchedule_MembershipApplications_ApplicationId",
                table: "VerificationSchedule",
                column: "ApplicationId",
                principalTable: "MembershipApplications",
                principalColumn: "MembershipApplicationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VerificationSchedule_VerificationRound_VerificationRoundId",
                table: "VerificationSchedule",
                column: "VerificationRoundId",
                principalTable: "VerificationRound",
                principalColumn: "VerificationRoundId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
