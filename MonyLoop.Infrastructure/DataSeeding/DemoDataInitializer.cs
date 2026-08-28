using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MonyLoop.Domain.Constants;
using MonyLoop.Domain.Constants.Agreement___Payment;
using MonyLoop.Domain.Constants.Onboarding___Member_Ledger;
using MonyLoop.Domain.Constants.Verification;
using MonyLoop.Domain.Entities.Agreement___Payment;
using MonyLoop.Domain.Entities.CircleRequestManagement;
using MonyLoop.Domain.Entities.Marketplace___Applications;
using MonyLoop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.UserAuth;
using MonyLoop.Domain.Entities.Verification;
using MonyLoop.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using OnboardingDocument = MonyLoop.Domain.Entities.Onboarding___Member_Ledger.Document;

namespace MonyLoop.Infrastructure.DataSeeding;

/// <summary>
/// Seeds a deterministic, end-to-end dataset for local UI and API testing.
/// This initializer is deliberately disabled outside the Development environment.
/// </summary>
public sealed class DemoDataInitializer
{
    private const string DemoPasswordFallback = "Demo123#";
    private const string AdminEmail = "mohamed.mohsenf23+admin@gmail.com";
    private const string OrganizerEmail = "mohamed.mohsenf23+organizer@gmail.com";
    private const string MemberEmail = "mohamed.mohsenf23+member@gmail.com";
    private const string OnboardingEmail = "mohamed.mohsenf23+onboarding@gmail.com";
    private const string AgreementEmail = "mohamed.mohsenf23+agreement@gmail.com";
    private const string ReadyEmail = "mohamed.mohsenf23+ready@gmail.com";
    private static readonly Guid NationalIdRequirementId = Guid.Parse("8F6A0F13-55F6-4D7E-B560-5C0D0C428A01");
    private static readonly Guid StarterRequestId = Guid.Parse("20000000-0000-0000-0000-000000000001");

    private readonly MonyLoopDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DemoDataOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DemoDataInitializer> _logger;

    public DemoDataInitializer(
        MonyLoopDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IOptions<DemoDataOptions> options,
        IHostEnvironment environment,
        IConfiguration configuration,
        ILogger<DemoDataInitializer> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _options = options.Value;
        _environment = environment;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!_environment.IsDevelopment() || !_options.Enabled)
        {
            _logger.LogInformation("Demo data seeding is disabled for environment {Environment}.", _environment.EnvironmentName);
            return;
        }

        var password = string.IsNullOrWhiteSpace(_options.Password)
            ? DemoPasswordFallback
            : _options.Password;

        var admin = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            AdminEmail, "Demo", "Admin", ApplicationRole.Admin,
            "29801010101010", password);
        var organizer = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000002"),
            OrganizerEmail, "Omar", "Organizer", ApplicationRole.Organizer,
            "29802020202020", password, admin.Id);
        var member = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000003"),
            MemberEmail, "Mona", "Member", ApplicationRole.Member,
            "29803030303030", password, admin.Id);
        var onboardingMember = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000004"),
            OnboardingEmail, "Nour", "Onboarding", ApplicationRole.Member,
            "29804040404040", password, admin.Id);
        var agreementMember = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000005"),
            AgreementEmail, "Ali", "Agreement", ApplicationRole.Member,
            "29805050505050", password, admin.Id);
        var readyMember = await EnsureUserAsync(
            Guid.Parse("10000000-0000-0000-0000-000000000006"),
            ReadyEmail, "Salma", "Ready", ApplicationRole.Member,
            "29806060606060", password, admin.Id);

        await ReconcileMembershipApplicationEmailsAsync(
            member.Id,
            onboardingMember.Id,
            agreementMember.Id,
            readyMember.Id,
            cancellationToken);
        await EnsureDemoFilesAsync(cancellationToken);
        await ReconcileDemoDocumentsAsync(cancellationToken);

        if (await _dbContext.CircleRequests.AnyAsync(x => x.RequestId == StarterRequestId, cancellationToken))
        {
            _logger.LogInformation("Demo workflow data already exists; domain seeding was skipped.");
            return;
        }

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        var approvedRequestId = Guid.Parse("20000000-0000-0000-0000-000000000002");
        var filledRequestId = Guid.Parse("20000000-0000-0000-0000-000000000003");
        var closedRequestId = Guid.Parse("20000000-0000-0000-0000-000000000004");
        var submittedRequestId = Guid.Parse("20000000-0000-0000-0000-000000000005");
        var submittedRequestTwoId = Guid.Parse("20000000-0000-0000-0000-000000000006");
        var modificationRequestId = Guid.Parse("20000000-0000-0000-0000-000000000007");
        var draftRequestId = Guid.Parse("20000000-0000-0000-0000-000000000008");
        var rejectedRequestId = Guid.Parse("20000000-0000-0000-0000-000000000009");

        var starterCircleId = Guid.Parse("30000000-0000-0000-0000-000000000001");
        var approvedCircleId = Guid.Parse("30000000-0000-0000-0000-000000000002");
        var filledCircleId = Guid.Parse("30000000-0000-0000-0000-000000000003");
        var closedCircleId = Guid.Parse("30000000-0000-0000-0000-000000000004");
        var starterListingId = Guid.Parse("40000000-0000-0000-0000-000000000001");

        _dbContext.CircleRequests.AddRange(
            Request(StarterRequestId, organizer.Id, admin.Id, "Cairo Growth Circle", 2500m, 8, CircleRequestStatus.Published, now.AddDays(-45), now.AddDays(-44), now.AddDays(-42), "Approved for the demo marketplace."),
            Request(approvedRequestId, organizer.Id, admin.Id, "Small Business Circle", 4000m, 6, CircleRequestStatus.Approved, now.AddDays(-20), now.AddDays(-19), now.AddDays(-18), "Approved and waiting for publication."),
            Request(filledRequestId, organizer.Id, admin.Id, "Family Goals Circle", 1500m, 5, CircleRequestStatus.Fulfilled, now.AddDays(-90), now.AddDays(-89), now.AddDays(-87), "All slots have been filled."),
            Request(closedRequestId, organizer.Id, admin.Id, "Completed Savings Circle", 1000m, 4, CircleRequestStatus.Fulfilled, now.AddDays(-180), now.AddDays(-179), now.AddDays(-177), "Circle completed successfully."),
            Request(submittedRequestId, organizer.Id, null, "Education Fund Circle", 3000m, 10, CircleRequestStatus.Submitted, now.AddDays(-3), now.AddDays(-2), null, null),
            Request(submittedRequestTwoId, organizer.Id, null, "Wedding Plan Circle", 5000m, 8, CircleRequestStatus.Submitted, now.AddDays(-2), now.AddDays(-1), null, null),
            Request(modificationRequestId, organizer.Id, admin.Id, "Home Upgrade Circle", 3500m, 7, CircleRequestStatus.ModificationRequested, now.AddDays(-12), now.AddDays(-11), now.AddDays(-10), "Please clarify the requested duration and slot plan."),
            Request(draftRequestId, organizer.Id, null, "Emergency Buffer Circle", 2000m, 6, CircleRequestStatus.Draft, now.AddDays(-1), null, null, null),
            Request(rejectedRequestId, organizer.Id, admin.Id, "High Risk Demo Circle", 25000m, 3, CircleRequestStatus.Rejected, now.AddDays(-30), now.AddDays(-29), now.AddDays(-28), "Contribution exceeds the MVP risk threshold."));

        _dbContext.Circles.AddRange(
            new Circle { CircleId = starterCircleId, RequestId = StarterRequestId, ApprovedSlots = 8, FilledCount = 1, Amount = 2500m, Duration = 8, Status = CircleStatus.InRecruitment },
            new Circle { CircleId = approvedCircleId, RequestId = approvedRequestId, ApprovedSlots = 6, FilledCount = 0, Amount = 4000m, Duration = 6, Status = CircleStatus.Open },
            new Circle { CircleId = filledCircleId, RequestId = filledRequestId, ApprovedSlots = 5, FilledCount = 5, Amount = 1500m, Duration = 5, Status = CircleStatus.Filled },
            new Circle { CircleId = closedCircleId, RequestId = closedRequestId, ApprovedSlots = 4, FilledCount = 4, Amount = 1000m, Duration = 4, Status = CircleStatus.Closed });

        _dbContext.MarketplaceListings.AddRange(
            new MarketplaceListing { ListingId = starterListingId, CircleId = starterCircleId, ListingStatus = MarketplaceListingStatus.Active },
            new MarketplaceListing { ListingId = Guid.Parse("40000000-0000-0000-0000-000000000002"), CircleId = filledCircleId, ListingStatus = MarketplaceListingStatus.Completed },
            new MarketplaceListing { ListingId = Guid.Parse("40000000-0000-0000-0000-000000000003"), CircleId = closedCircleId, ListingStatus = MarketplaceListingStatus.Cancelled });

        var activeApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000001");
        var scheduledApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000004");
        var completedApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000005");
        var agreementApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000006");
        var onboardingApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000007");
        var readyApplicationId = Guid.Parse("50000000-0000-0000-0000-000000000008");

        _dbContext.MembershipApplications.AddRange(
            Application(activeApplicationId, member.Id, starterListingId, "Mona Member", MemberEmail, "01010000001", "29803030303030", MembershipApplicationStage.Confirmed, now.AddDays(-35)),
            Application(Guid.Parse("50000000-0000-0000-0000-000000000002"), null, starterListingId, "Guest Submitted", "submitted@example.test", "01010000002", "29807070707070", MembershipApplicationStage.Submitted, now.AddDays(-1)),
            Application(Guid.Parse("50000000-0000-0000-0000-000000000003"), null, starterListingId, "Guest Shortlisted", "shortlisted@example.test", "01010000003", "29808080808080", MembershipApplicationStage.Shortlisted, now.AddDays(-3)),
            Application(scheduledApplicationId, null, starterListingId, "Guest Scheduled", "scheduled@example.test", "01010000004", "29809090909090", MembershipApplicationStage.VerificationScheduled, now.AddDays(-7)),
            Application(completedApplicationId, null, starterListingId, "Guest Verified", "verified@example.test", "01010000005", "29810101010101", MembershipApplicationStage.VerificationCompleted, now.AddDays(-12)),
            Application(agreementApplicationId, agreementMember.Id, starterListingId, "Ali Agreement", AgreementEmail, "01010000006", "29805050505050", MembershipApplicationStage.AgreementExtended, now.AddDays(-10)),
            Application(onboardingApplicationId, onboardingMember.Id, starterListingId, "Nour Onboarding", OnboardingEmail, "01010000007", "29804040404040", MembershipApplicationStage.Confirmed, now.AddDays(-18)),
            Application(readyApplicationId, readyMember.Id, starterListingId, "Salma Ready", ReadyEmail, "01010000008", "29806060606060", MembershipApplicationStage.Confirmed, now.AddDays(-22)),
            Application(Guid.Parse("50000000-0000-0000-0000-000000000009"), null, starterListingId, "Guest Rejected", "rejected@example.test", "01010000009", "29811111111111", MembershipApplicationStage.Rejected, now.AddDays(-16)));

        var roundId = Guid.Parse("60000000-0000-0000-0000-000000000001");
        var criterionIdentityId = Guid.Parse("61000000-0000-0000-0000-000000000001");
        var criterionAbilityId = Guid.Parse("61000000-0000-0000-0000-000000000002");
        var scheduledScheduleId = Guid.Parse("62000000-0000-0000-0000-000000000001");
        var completedScheduleId = Guid.Parse("62000000-0000-0000-0000-000000000002");
        var submissionId = Guid.Parse("63000000-0000-0000-0000-000000000001");

        _dbContext.VerificationRounds.Add(new VerificationRound
        {
            VerificationRoundId = roundId,
            CircleId = starterCircleId,
            ReviewedByUserId = organizer.Id,
            RoundName = "Starter member verification",
            Format = VerificationFormat.Video
        });
        _dbContext.VerificationCriteria.AddRange(
            new VerificationCriterion { VerificationCriterionId = criterionIdentityId, VerificationRoundId = roundId, CriterionName = "Identity and contact validation", Weight = 40m, DisplayOrder = 1, IsActive = true },
            new VerificationCriterion { VerificationCriterionId = criterionAbilityId, VerificationRoundId = roundId, CriterionName = "Contribution ability", Weight = 60m, DisplayOrder = 2, IsActive = true });
        _dbContext.VerificationSchedules.AddRange(
            new VerificationSchedule { VerificationScheduleId = scheduledScheduleId, ApplicationId = scheduledApplicationId, VerificationRoundId = roundId, Date = today.AddDays(2), Time = new TimeOnly(15, 0), VideoLink = "https://meet.example.test/demo-scheduled", Status = ScheduleStatus.Scheduled },
            new VerificationSchedule { VerificationScheduleId = completedScheduleId, ApplicationId = completedApplicationId, VerificationRoundId = roundId, Date = today.AddDays(-5), Time = new TimeOnly(11, 30), VideoLink = "https://meet.example.test/demo-completed", Status = ScheduleStatus.Completed });
        _dbContext.VerificationChecklistSubmissions.Add(new VerificationChecklistSubmission
        {
            VerificationChecklistSubmissionId = submissionId,
            VerificationScheduleId = completedScheduleId,
            SubmittedByUserId = organizer.Id,
            CompositeScore = 88m,
            OverallComments = "Identity verified and contribution plan is realistic.",
            SubmittedAt = now.AddDays(-5)
        });
        _dbContext.VerificationCriterionRatings.AddRange(
            new VerificationCriterionRating { VerificationCriterionRatingId = Guid.Parse("64000000-0000-0000-0000-000000000001"), VerificationChecklistSubmissionId = submissionId, VerificationCriterionId = criterionIdentityId, Rating = 5, Comments = "Documents and contact details match." },
            new VerificationCriterionRating { VerificationCriterionRatingId = Guid.Parse("64000000-0000-0000-0000-000000000002"), VerificationChecklistSubmissionId = submissionId, VerificationCriterionId = criterionAbilityId, Rating = 4, Comments = "Stable monthly contribution capacity." });

        var activeAgreementId = Guid.Parse("70000000-0000-0000-0000-000000000001");
        var pendingAgreementId = Guid.Parse("70000000-0000-0000-0000-000000000002");
        var onboardingAgreementId = Guid.Parse("70000000-0000-0000-0000-000000000003");
        var readyAgreementId = Guid.Parse("70000000-0000-0000-0000-000000000004");
        _dbContext.MembershipAgreements.AddRange(
            Agreement(activeAgreementId, activeApplicationId, "Mona Member", "Cairo Growth Circle", 1, today.AddDays(-32), AgreementStatus.Accepted, now.AddDays(-33), now.AddDays(-32), 'a'),
            Agreement(pendingAgreementId, agreementApplicationId, "Ali Agreement", "Cairo Growth Circle", 4, today.AddDays(-9), AgreementStatus.Pending, now.AddDays(-9), null, 'b'),
            Agreement(onboardingAgreementId, onboardingApplicationId, "Nour Onboarding", "Cairo Growth Circle", 3, today.AddDays(-16), AgreementStatus.Accepted, now.AddDays(-17), now.AddDays(-16), 'c'),
            Agreement(readyAgreementId, readyApplicationId, "Salma Ready", "Cairo Growth Circle", 5, today.AddDays(-20), AgreementStatus.Accepted, now.AddDays(-21), now.AddDays(-20), 'd'));

        var activeOnboardingId = Guid.Parse("80000000-0000-0000-0000-000000000001");
        var onboardingCaseId = Guid.Parse("80000000-0000-0000-0000-000000000002");
        var readyOnboardingId = Guid.Parse("80000000-0000-0000-0000-000000000003");
        _dbContext.OnboardingCases.AddRange(
            new OnboardingCase { OnboardingCaseId = activeOnboardingId, MembershipAgreementId = activeAgreementId, OrganizerId = organizer.Id, UserId = member.Id, FinalStatus = OnboardingCaseStatus.Activated, CreatedAt = now.AddDays(-32) },
            new OnboardingCase { OnboardingCaseId = onboardingCaseId, MembershipAgreementId = onboardingAgreementId, OrganizerId = organizer.Id, UserId = onboardingMember.Id, FinalStatus = OnboardingCaseStatus.InProgress, CreatedAt = now.AddDays(-16) },
            new OnboardingCase { OnboardingCaseId = readyOnboardingId, MembershipAgreementId = readyAgreementId, OrganizerId = organizer.Id, UserId = readyMember.Id, FinalStatus = OnboardingCaseStatus.Approved, CreatedAt = now.AddDays(-20) });

        var activeLedgerId = Guid.Parse("90000000-0000-0000-0000-000000000001");
        _dbContext.MemberLedgers.Add(new MemberLedger
        {
            MemberLedgerId = activeLedgerId,
            UserId = member.Id,
            OnboardingCaseId = activeOnboardingId,
            ActivatedByAdminId = admin.Id,
            ActivatedAt = now.AddDays(-30)
        });

        _dbContext.CircleSlots.AddRange(CreateSlots(starterCircleId, 8, "31000000-0000-0000-0000-0000000000", activeLedgerId, now.AddDays(-30)));
        _dbContext.CircleSlots.AddRange(CreateSlots(approvedCircleId, 6, "32000000-0000-0000-0000-0000000000"));
        _dbContext.CircleSlots.AddRange(CreateSlots(filledCircleId, 5, "33000000-0000-0000-0000-0000000000", locked: true));
        _dbContext.CircleSlots.AddRange(CreateSlots(closedCircleId, 4, "34000000-0000-0000-0000-0000000000", locked: true));

        _dbContext.Documents.AddRange(
            DemoDocument(Guid.Parse("81000000-0000-0000-0000-000000000001"), activeOnboardingId, "active-member-national-id.pdf", "demo/active-member-national-id.pdf", DocumentStatus.Approved, now.AddDays(-31), admin.Id, now.AddDays(-30)),
            DemoDocument(Guid.Parse("81000000-0000-0000-0000-000000000002"), onboardingCaseId, "onboarding-national-id.pdf", "demo/onboarding-national-id.pdf", DocumentStatus.Pending, now.AddDays(-1), null, null),
            DemoDocument(Guid.Parse("81000000-0000-0000-0000-000000000003"), readyOnboardingId, "ready-member-national-id.pdf", "demo/ready-member-national-id.pdf", DocumentStatus.Approved, now.AddDays(-19), organizer.Id, now.AddDays(-18)));

        _dbContext.PaymentTransactions.AddRange(
            Payment(Guid.Parse("A0000000-0000-0000-0000-000000000001"), activeLedgerId, starterCircleId, organizer.Id, PaymentTransactionType.PayIn, PaymentTransactionStatus.Successful, 2500m, now.AddDays(-28), "DEMO-PAYIN-001"),
            Payment(Guid.Parse("A0000000-0000-0000-0000-000000000002"), activeLedgerId, starterCircleId, organizer.Id, PaymentTransactionType.PayIn, PaymentTransactionStatus.Successful, 2500m, now.AddDays(-14), "DEMO-PAYIN-002"),
            Payment(Guid.Parse("A0000000-0000-0000-0000-000000000003"), activeLedgerId, starterCircleId, organizer.Id, PaymentTransactionType.PayIn, PaymentTransactionStatus.Successful, 2500m, now.AddDays(-2), "DEMO-PAYIN-003"),
            Payment(Guid.Parse("A0000000-0000-0000-0000-000000000004"), activeLedgerId, starterCircleId, organizer.Id, PaymentTransactionType.PayOut, PaymentTransactionStatus.Pending, 17500m, now.AddDays(5), "DEMO-PAYOUT-001"),
            Payment(Guid.Parse("A0000000-0000-0000-0000-000000000005"), activeLedgerId, starterCircleId, organizer.Id, PaymentTransactionType.PayIn, PaymentTransactionStatus.Failed, 2500m, now.AddDays(-7), "DEMO-FAILED-001"));

        _dbContext.AuditLogs.AddRange(
            Audit(Guid.Parse("B0000000-0000-0000-0000-000000000001"), StarterRequestId, organizer.Id, "Submitted", "Draft", "Submitted", now.AddDays(-44)),
            Audit(Guid.Parse("B0000000-0000-0000-0000-000000000002"), StarterRequestId, admin.Id, "Approved", "Submitted", "Approved", now.AddDays(-42)),
            Audit(Guid.Parse("B0000000-0000-0000-0000-000000000003"), StarterRequestId, organizer.Id, "Published", "Approved", "Published", now.AddDays(-41)),
            Audit(Guid.Parse("B0000000-0000-0000-0000-000000000004"), modificationRequestId, admin.Id, "ModificationRequested", "Submitted", "ModificationRequested", now.AddDays(-10)));

        await _dbContext.SaveChangesAsync(cancellationToken);
        await ReconcileDemoDocumentsAsync(cancellationToken);

        _logger.LogInformation(
            "Demo data seeded with Gmail plus-alias accounts for admin, organizer, member, onboarding, agreement, and ready scenarios.");
    }

    private async Task<ApplicationUser> EnsureUserAsync(
        Guid preferredId,
        string email,
        string firstName,
        string lastName,
        string role,
        string nationalId,
        string password,
        Guid? registeredByAdminId = null)
    {
        var existing = await _userManager.FindByEmailAsync(email)
            ?? await _userManager.FindByIdAsync(preferredId.ToString());
        if (existing is not null)
        {
            if (!string.Equals(existing.Email, email, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(existing.UserName, email, StringComparison.OrdinalIgnoreCase))
            {
                existing.Email = email;
                existing.UserName = email;
                existing.EmailConfirmed = true;
                existing.MustChangePassword = false;

                var updateResult = await _userManager.UpdateAsync(existing);
                EnsureIdentitySucceeded(updateResult, $"update demo email to {email}");
            }

            if (!await _userManager.IsInRoleAsync(existing, role))
            {
                var roleResult = await _userManager.AddToRoleAsync(existing, role);
                EnsureIdentitySucceeded(roleResult, $"assign role {role} to {email}");
            }

            return existing;
        }

        var user = new ApplicationUser
        {
            Id = preferredId,
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            NationalId = nationalId,
            EmailConfirmed = true,
            MustChangePassword = false,
            RegisteredByAdminId = registeredByAdminId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };

        var createResult = await _userManager.CreateAsync(user, password);
        EnsureIdentitySucceeded(createResult, $"create {email}");
        var addRoleResult = await _userManager.AddToRoleAsync(user, role);
        EnsureIdentitySucceeded(addRoleResult, $"assign role {role} to {email}");
        return user;
    }

    private async Task ReconcileMembershipApplicationEmailsAsync(
        Guid memberId,
        Guid onboardingMemberId,
        Guid agreementMemberId,
        Guid readyMemberId,
        CancellationToken cancellationToken)
    {
        var emailsByUserId = new Dictionary<Guid, string>
        {
            [memberId] = MemberEmail,
            [onboardingMemberId] = OnboardingEmail,
            [agreementMemberId] = AgreementEmail,
            [readyMemberId] = ReadyEmail
        };

        var userIds = emailsByUserId.Keys.ToArray();
        var applications = await _dbContext.MembershipApplications
            .Where(x => x.UserId.HasValue && userIds.Contains(x.UserId.Value))
            .ToListAsync(cancellationToken);

        var changed = false;
        foreach (var application in applications)
        {
            var expectedEmail = emailsByUserId[application.UserId!.Value];
            if (string.Equals(application.Email, expectedEmail, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            application.Email = expectedEmail;
            application.UpdatedAt = DateTime.UtcNow;
            changed = true;
        }

        if (changed)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static void EnsureIdentitySucceeded(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Could not {operation}: {string.Join(", ", result.Errors.Select(x => x.Description))}");
        }
    }

    private async Task EnsureDemoFilesAsync(CancellationToken cancellationToken)
    {
        var demoFolder = Path.Combine(ResolveStorageRoot(), "demo");
        Directory.CreateDirectory(demoFolder);

        var files = new Dictionary<string, string>
        {
            ["active-member-national-id.pdf"] = "Approved National ID placeholder for Mona Member",
            ["onboarding-national-id.pdf"] = "Pending National ID placeholder for Nour Onboarding",
            ["ready-member-national-id.pdf"] = "Approved National ID placeholder for Salma Ready"
        };

        foreach (var (fileName, description) in files)
        {
            var path = Path.Combine(demoFolder, fileName);
            if (!File.Exists(path))
            {
                await File.WriteAllBytesAsync(path, GenerateDemoPdf(description), cancellationToken);
            }
        }
    }

    private async Task ReconcileDemoDocumentsAsync(CancellationToken cancellationToken)
    {
        var expectedFiles = new Dictionary<Guid, string>
        {
            [Guid.Parse("81000000-0000-0000-0000-000000000001")] = "active-member-national-id.pdf",
            [Guid.Parse("81000000-0000-0000-0000-000000000002")] = "onboarding-national-id.pdf",
            [Guid.Parse("81000000-0000-0000-0000-000000000003")] = "ready-member-national-id.pdf"
        };
        var documentIds = expectedFiles.Keys.ToArray();
        var documents = await _dbContext.Documents
            .Where(x => documentIds.Contains(x.DocumentId))
            .ToListAsync(cancellationToken);

        var changed = false;
        foreach (var document in documents)
        {
            var expectedName = expectedFiles[document.DocumentId];
            var expectedPath = $"demo/{expectedName}";
            var fileInfo = new FileInfo(Path.Combine(ResolveStorageRoot(), "demo", expectedName));

            if (document.FileName == expectedName &&
                document.FilePath == expectedPath &&
                document.FileSize == fileInfo.Length)
            {
                continue;
            }

            document.FileName = expectedName;
            document.FilePath = expectedPath;
            document.FileSize = fileInfo.Length;
            changed = true;
        }

        if (changed)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private string ResolveStorageRoot()
    {
        var rootPath = _configuration["FileStorage:RootPath"];
        return string.IsNullOrWhiteSpace(rootPath)
            ? Path.Combine(_environment.ContentRootPath, "AppFiles")
            : Path.IsPathRooted(rootPath)
                ? rootPath
                : Path.Combine(_environment.ContentRootPath, rootPath);
    }

    private static byte[] GenerateDemoPdf(string description)
    {
        return QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(48);
                page.Header().Text("AMAN MONEY LOOP - DEMO DOCUMENT").Bold().FontSize(20);
                page.Content().PaddingVertical(24).Column(column =>
                {
                    column.Spacing(12);
                    column.Item().Text(description).FontSize(14);
                    column.Item().Text("This PDF contains dummy data and is intended only for local workflow testing.");
                });
                page.Footer().AlignCenter().Text("Demo fixture - not a real identity document");
            });
        }).GeneratePdf();
    }

    private static CircleRequest Request(
        Guid id, Guid organizerId, Guid? adminId, string title, decimal amount, int slots,
        CircleRequestStatus status, DateTime createdAt, DateTime? submittedAt,
        DateTime? reviewedAt, string? reason) => new()
    {
        RequestId = id,
        CreatedByOrganizerId = organizerId,
        ReviewedByAdminId = adminId,
        CircleTitle = title,
        CircleType = CircleType.NewCircle,
        ContributionAmount = amount,
        Duration = slots,
        NumberOfSlots = slots,
        ShortJustification = $"Demo request for {title}.",
        RequestStatus = status,
        CreatedAt = createdAt,
        SubmittedAt = submittedAt,
        ReviewedAt = reviewedAt,
        DecisionReason = reason
    };

    private static MembershipApplication Application(
        Guid id, Guid? userId, Guid listingId, string name, string email, string phone,
        string nationalId, MembershipApplicationStage stage, DateTime createdAt) => new()
    {
        MembershipApplicationId = id,
        UserId = userId,
        ListingId = listingId,
        Name = name,
        Email = email,
        Phone = phone,
        NationalId = nationalId,
        Stage = stage,
        CreatedAt = createdAt,
        UpdatedAt = createdAt.AddHours(8)
    };

    private static MembershipAgreement Agreement(
        Guid id, Guid applicationId, string memberName, string circleTitle, int payoutSlot,
        DateOnly startDate, AgreementStatus status, DateTime createdAt, DateTime? respondedAt,
        char tokenCharacter) => new()
    {
        MembershipAgreementId = id,
        MembershipApplicationId = applicationId,
        MemberName = memberName,
        CircleTitle = circleTitle,
        ContributionSchedule = "EGP 2,500 monthly for 8 months",
        PayoutSlot = payoutSlot,
        StartDate = startDate,
        ExpiryDate = startDate.AddDays(7),
        Status = status,
        ResponseTokenHash = new string(tokenCharacter, 64),
        CreatedAt = createdAt,
        RespondedAt = respondedAt
    };

    private static IEnumerable<CircleSlot> CreateSlots(
        Guid circleId, int count, string idPrefix, Guid? memberLedgerId = null,
        DateTime? assignedAt = null, bool locked = false)
    {
        for (var slotNumber = 1; slotNumber <= count; slotNumber++)
        {
            var id = Guid.Parse($"{idPrefix}{slotNumber:X2}");
            yield return new CircleSlot
            {
                CircleSlotId = id,
                CircleId = circleId,
                MemberLedgerId = slotNumber == 1 ? memberLedgerId : null,
                SlotNumber = slotNumber,
                Status = locked
                    ? CircleSlotStatus.Locked
                    : slotNumber == 1 && memberLedgerId.HasValue
                        ? CircleSlotStatus.Assigned
                        : CircleSlotStatus.Vacant,
                AssignedAt = slotNumber == 1 ? assignedAt : null
            };
        }
    }

    private static OnboardingDocument DemoDocument(
        Guid id, Guid caseId, string fileName, string filePath, DocumentStatus status,
        DateTime uploadedAt, Guid? reviewedBy, DateTime? reviewedAt) => new()
    {
        DocumentId = id,
        OnboardingCaseId = caseId,
        DocumentRequirementId = NationalIdRequirementId,
        ReviewedByUserId = reviewedBy,
        FileName = fileName,
        FilePath = filePath,
        FileSize = 64,
        Status = status,
        UploadedAt = uploadedAt,
        ReviewedAt = reviewedAt
    };

    private static PaymentTransaction Payment(
        Guid id, Guid ledgerId, Guid circleId, Guid recordedBy, PaymentTransactionType type,
        PaymentTransactionStatus status, decimal amount, DateTime transactionDate, string reference) => new()
    {
        PaymentTransactionId = id,
        MemberLedgerId = ledgerId,
        CircleId = circleId,
        RecordedByUserId = recordedBy,
        TransactionType = type,
        PaymentMethod = PaymentMethod.EWallet,
        TransactionStatus = status,
        Amount = amount,
        TransactionReference = reference,
        ReceiptNumber = $"R-{reference}",
        TransactionDate = transactionDate,
        CreatedAt = transactionDate
    };

    private static AuditLog Audit(
        Guid id, Guid entityId, Guid userId, string action, string oldStatus,
        string newStatus, DateTime createdAt) => new()
    {
        AuditLogId = id,
        EntityId = entityId,
        PerformedByUserId = userId,
        EntityType = nameof(CircleRequest),
        ActionType = action,
        OldStatus = oldStatus,
        NewStatus = newStatus,
        ActionDescription = $"Demo lifecycle action: {action}.",
        CreatedAt = createdAt
    };
}
