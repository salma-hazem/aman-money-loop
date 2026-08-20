using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonyLoop.Infrastructure.Data.MigrationsV2
{
    /// <inheritdoc />
    public partial class InitialCreateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentRequirements",
                columns: table => new
                {
                    DocumentRequirementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentRequirements", x => x.DocumentRequirementId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RegisteredByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_RegisteredByAdminId",
                        column: x => x.RegisteredByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PerformedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActionDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogId);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_PerformedByUserId",
                        column: x => x.PerformedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OTPTokens",
                columns: table => new
                {
                    OTPTokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AttemptsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTPTokens", x => x.OTPTokenId);
                    table.ForeignKey(
                        name: "FK_OTPTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CircleRequests",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExistingCircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByOrganizerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CircleTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CircleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "NewCircle"),
                    ContributionAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    NumberOfSlots = table.Column<int>(type: "int", nullable: false),
                    ShortJustification = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    VacantSlotNumber = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_CircleRequests_Users_CreatedByOrganizerId",
                        column: x => x.CreatedByOrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CircleRequests_Users_ReviewedByAdminId",
                        column: x => x.ReviewedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Circles",
                columns: table => new
                {
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedSlots = table.Column<int>(type: "int", nullable: false),
                    FilledCount = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Open")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circles", x => x.CircleId);
                    table.ForeignKey(
                        name: "FK_Circles_CircleRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CircleRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MarketplaceListings",
                columns: table => new
                {
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceListings", x => x.ListingId);
                    table.ForeignKey(
                        name: "FK_MarketplaceListings_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "CircleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationRounds",
                columns: table => new
                {
                    VerificationRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoundName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRounds", x => x.VerificationRoundId);
                    table.ForeignKey(
                        name: "FK_VerificationRounds_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "CircleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MembershipApplications",
                columns: table => new
                {
                    MembershipApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Submitted"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipApplications", x => x.MembershipApplicationId);
                    table.ForeignKey(
                        name: "FK_MembershipApplications_MarketplaceListings_ListingId",
                        column: x => x.ListingId,
                        principalTable: "MarketplaceListings",
                        principalColumn: "ListingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCriteria",
                columns: table => new
                {
                    VerificationCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCriteria", x => x.VerificationCriterionId);
                    table.ForeignKey(
                        name: "FK_VerificationCriteria_VerificationRounds_VerificationRoundId",
                        column: x => x.VerificationRoundId,
                        principalTable: "VerificationRounds",
                        principalColumn: "VerificationRoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MembershipAgreements",
                columns: table => new
                {
                    MembershipAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CircleTitle = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ContributionSchedule = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PayoutSlot = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ResponseTokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipAgreements", x => x.MembershipAgreementId);
                    table.ForeignKey(
                        name: "FK_MembershipAgreements_MembershipApplications_MembershipApplicationId",
                        column: x => x.MembershipApplicationId,
                        principalTable: "MembershipApplications",
                        principalColumn: "MembershipApplicationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationSchedules",
                columns: table => new
                {
                    VerificationScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    LocationLink = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    VideoLink = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationSchedules", x => x.VerificationScheduleId);
                    table.ForeignKey(
                        name: "FK_VerificationSchedules_MembershipApplications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "MembershipApplications",
                        principalColumn: "MembershipApplicationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VerificationSchedules_VerificationRounds_VerificationRoundId",
                        column: x => x.VerificationRoundId,
                        principalTable: "VerificationRounds",
                        principalColumn: "VerificationRoundId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnboardingCases",
                columns: table => new
                {
                    OnboardingCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipAgreementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnboardingCases", x => x.OnboardingCaseId);
                    table.ForeignKey(
                        name: "FK_OnboardingCases_MembershipAgreements_MembershipAgreementId",
                        column: x => x.MembershipAgreementId,
                        principalTable: "MembershipAgreements",
                        principalColumn: "MembershipAgreementId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OnboardingCases_Users_OrganizerId",
                        column: x => x.OrganizerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationChecklistSubmissions",
                columns: table => new
                {
                    VerificationChecklistSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmittedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompositeScore = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    OverallComments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationChecklistSubmissions", x => x.VerificationChecklistSubmissionId);
                    table.ForeignKey(
                        name: "FK_VerificationChecklistSubmissions_VerificationSchedules_VerificationScheduleId",
                        column: x => x.VerificationScheduleId,
                        principalTable: "VerificationSchedules",
                        principalColumn: "VerificationScheduleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentRequirementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentRequirements_DocumentRequirementId",
                        column: x => x.DocumentRequirementId,
                        principalTable: "DocumentRequirements",
                        principalColumn: "DocumentRequirementId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_OnboardingCases_OnboardingCaseId",
                        column: x => x.OnboardingCaseId,
                        principalTable: "OnboardingCases",
                        principalColumn: "OnboardingCaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_Users_ReviewedByUserId",
                        column: x => x.ReviewedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MemberLedgers",
                columns: table => new
                {
                    MemberLedgerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OnboardingCaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberLedgers", x => x.MemberLedgerId);
                    table.ForeignKey(
                        name: "FK_MemberLedgers_OnboardingCases_OnboardingCaseId",
                        column: x => x.OnboardingCaseId,
                        principalTable: "OnboardingCases",
                        principalColumn: "OnboardingCaseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberLedgers_Users_ActivatedByAdminId",
                        column: x => x.ActivatedByAdminId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MemberLedgers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCriterionRatings",
                columns: table => new
                {
                    VerificationCriterionRatingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationChecklistSubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VerificationCriterionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCriterionRatings", x => x.VerificationCriterionRatingId);
                    table.CheckConstraint("CK_VerificationCriterionRating_Rating", "[Rating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_VerificationCriterionRatings_VerificationChecklistSubmissions_VerificationChecklistSubmissionId",
                        column: x => x.VerificationChecklistSubmissionId,
                        principalTable: "VerificationChecklistSubmissions",
                        principalColumn: "VerificationChecklistSubmissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VerificationCriterionRatings_VerificationCriteria_VerificationCriterionId",
                        column: x => x.VerificationCriterionId,
                        principalTable: "VerificationCriteria",
                        principalColumn: "VerificationCriterionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CircleSlots",
                columns: table => new
                {
                    CircleSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberLedgerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SlotNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Vacant"),
                    VacatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CircleSlots", x => x.CircleSlotId);
                    table.ForeignKey(
                        name: "FK_CircleSlots_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "CircleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CircleSlots_MemberLedgers_MemberLedgerId",
                        column: x => x.MemberLedgerId,
                        principalTable: "MemberLedgers",
                        principalColumn: "MemberLedgerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberLedgerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CircleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiptNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReceiptFilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.PaymentTransactionId);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Circles_CircleId",
                        column: x => x.CircleId,
                        principalTable: "Circles",
                        principalColumn: "CircleId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_MemberLedgers_MemberLedgerId",
                        column: x => x.MemberLedgerId,
                        principalTable: "MemberLedgers",
                        principalColumn: "MemberLedgerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedByUserId",
                table: "AuditLogs",
                column: "PerformedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleRequests_CreatedByOrganizerId",
                table: "CircleRequests",
                column: "CreatedByOrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleRequests_ExistingCircleId",
                table: "CircleRequests",
                column: "ExistingCircleId");

            migrationBuilder.CreateIndex(
                name: "IX_CircleRequests_RequestStatus",
                table: "CircleRequests",
                column: "RequestStatus");

            migrationBuilder.CreateIndex(
                name: "IX_CircleRequests_ReviewedByAdminId",
                table: "CircleRequests",
                column: "ReviewedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Circles_RequestId",
                table: "Circles",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Circles_Status",
                table: "Circles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CircleSlots_CircleId_SlotNumber",
                table: "CircleSlots",
                columns: new[] { "CircleId", "SlotNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CircleSlots_MemberLedgerId",
                table: "CircleSlots",
                column: "MemberLedgerId",
                unique: true,
                filter: "[MemberLedgerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CircleSlots_Status",
                table: "CircleSlots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentRequirementId",
                table: "Documents",
                column: "DocumentRequirementId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_OnboardingCaseId",
                table: "Documents",
                column: "OnboardingCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ReviewedByUserId",
                table: "Documents",
                column: "ReviewedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_CircleId",
                table: "MarketplaceListings",
                column: "CircleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceListings_ListingStatus",
                table: "MarketplaceListings",
                column: "ListingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgers_ActivatedByAdminId",
                table: "MemberLedgers",
                column: "ActivatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgers_OnboardingCaseId",
                table: "MemberLedgers",
                column: "OnboardingCaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberLedgers_UserId",
                table: "MemberLedgers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipAgreements_MembershipApplicationId",
                table: "MembershipAgreements",
                column: "MembershipApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MembershipApplications_ListingId",
                table: "MembershipApplications",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipApplications_Stage",
                table: "MembershipApplications",
                column: "Stage");

            migrationBuilder.CreateIndex(
                name: "IX_MembershipApplications_UserId",
                table: "MembershipApplications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingCases_MembershipAgreementId",
                table: "OnboardingCases",
                column: "MembershipAgreementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OnboardingCases_OrganizerId",
                table: "OnboardingCases",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_OTPTokens_UserId_Purpose",
                table: "OTPTokens",
                columns: new[] { "UserId", "Purpose" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_CircleId",
                table: "PaymentTransactions",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_MemberLedgerId",
                table: "PaymentTransactions",
                column: "MemberLedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_RecordedByUserId",
                table: "PaymentTransactions",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegisteredByAdminId",
                table: "Users",
                column: "RegisteredByAdminId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationChecklistSubmissions_VerificationScheduleId",
                table: "VerificationChecklistSubmissions",
                column: "VerificationScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCriteria_VerificationRoundId",
                table: "VerificationCriteria",
                column: "VerificationRoundId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCriterionRatings_VerificationChecklistSubmissionId_VerificationCriterionId",
                table: "VerificationCriterionRatings",
                columns: new[] { "VerificationChecklistSubmissionId", "VerificationCriterionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCriterionRatings_VerificationCriterionId",
                table: "VerificationCriterionRatings",
                column: "VerificationCriterionId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRounds_CircleId",
                table: "VerificationRounds",
                column: "CircleId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationSchedules_ApplicationId",
                table: "VerificationSchedules",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationSchedules_VerificationRoundId",
                table: "VerificationSchedules",
                column: "VerificationRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_CircleRequests_Circles_ExistingCircleId",
                table: "CircleRequests",
                column: "ExistingCircleId",
                principalTable: "Circles",
                principalColumn: "CircleId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CircleRequests_Users_CreatedByOrganizerId",
                table: "CircleRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleRequests_Users_ReviewedByAdminId",
                table: "CircleRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_CircleRequests_Circles_ExistingCircleId",
                table: "CircleRequests");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CircleSlots");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "OTPTokens");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "VerificationCriterionRatings");

            migrationBuilder.DropTable(
                name: "DocumentRequirements");

            migrationBuilder.DropTable(
                name: "MemberLedgers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "VerificationChecklistSubmissions");

            migrationBuilder.DropTable(
                name: "VerificationCriteria");

            migrationBuilder.DropTable(
                name: "OnboardingCases");

            migrationBuilder.DropTable(
                name: "VerificationSchedules");

            migrationBuilder.DropTable(
                name: "MembershipAgreements");

            migrationBuilder.DropTable(
                name: "VerificationRounds");

            migrationBuilder.DropTable(
                name: "MembershipApplications");

            migrationBuilder.DropTable(
                name: "MarketplaceListings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Circles");

            migrationBuilder.DropTable(
                name: "CircleRequests");
        }
    }
}
