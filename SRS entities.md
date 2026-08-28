# SRS Entities - Aman Money Loop MVP

This file reflects the current Domain model plus the agreed corrections needed for the MVP Onion Architecture implementation.

## Module 1 - User and Account Management

Implementation note: Module 1 uses ASP.NET Core Identity with `Guid` user and role keys. `ApplicationUser` extends `IdentityUser<Guid>`, and Identity owns password hashing, normalized email/user names, lockout data, security stamps, role mapping, claims, logins, and tokens.

### ApplicationUser

- UserId: Guid
- FirstName: string
- LastName: string
- Email: string
- PasswordHash: string
- PhoneNumber: string
- NationalId: string?
- ProfilePictureUrl: string?
- PendingEmail: string?
- MustChangePassword: bool
- RegisteredByAdminId: Guid?
- CreatedAt: DateTime
- UpdatedAt: DateTime?
- IsActive: bool

Note: `PendingEmail` supports OTP verification before an email change is committed. Public members choose their password. Admin-created Organizer/Admin accounts receive a temporary password and must replace it on first login.

### ApplicationRole

- RoleId: Guid
- RoleName: string

Canonical roles are `Admin`, `Organizer`, and `Member`.

### Identity UserRole

- UserId: Guid
- RoleId: Guid

### OtpToken

- OtpTokenId: Guid
- UserId: Guid
- Code: string
- Purpose: OTPPurpose
- IsUsed: bool
- AttemptsCount: int
- ExpiresAt: DateTime
- CreatedAt: DateTime

Note: The six-digit OTP is stored with its purpose, expiry, attempts, and single-use state. Redis limits OTP requests to one per minute. Supported purposes are registration confirmation, password reset, and email change.

### RefreshToken

- RefreshTokenId: Guid
- UserId: Guid
- Token: string
- ExpiresAt: DateTime
- CreatedAt: DateTime
- IsRevoked: bool
- RevokedAt: DateTime?
- ReplacedByToken: string?

Note: Refresh tokens rotate when used and are revoked after password reset or password change.

## Module 2 - Circle Request and Configuration Management

### CircleRequest

- RequestId: Guid
- ExistingCircleId: Guid?
- CreatedByOrganizerId: Guid
- ReviewedByAdminId: Guid?
- CircleTitle: string
- CircleType: CircleType
- ContributionAmount: decimal
- Duration: int
- NumberOfSlots: int
- ShortJustification: string?
- RequestStatus: CircleRequestStatus
- VacantSlotNumber: int?
- CreatedAt: DateTime
- SubmittedAt: DateTime?
- ReviewedAt: DateTime?
- DecisionReason: string?

Lifecycle note: new requests store organizer-entered terms. Replacement requests store `ExistingCircleId` and `VacantSlotNumber`; their title, amount, duration, and one-slot count are derived from the existing circle. Requests are retained and cancelled rather than physically deleted.

### Circle

- CircleId: Guid
- RequestId: Guid
- ApprovedSlots: int
- FilledCount: int
- Amount: decimal
- Duration: int
- Status: CircleStatus

Lifecycle note: approval creates the circle as `Open`; publication changes it to `InRecruitment`; filling its final slot changes it to `Filled`; cancellation of its original published request changes it to `Closed`.

### MarketplaceListing

- ListingId: Guid
- CircleId: Guid
- ListingStatus: MarketplaceListingStatus

### CircleSlot

- CircleSlotId: Guid
- CircleId: Guid
- MemberLedgerId: Guid?
- SlotNumber: int
- Status: CircleSlotStatus
- VacatedAt: DateTime?
- AssignedAt: DateTime?

Lifecycle note: a new slot starts `Vacant`. Assignment sets `MemberLedgerId`, `Status = Assigned`, and `AssignedAt`. Vacancy clears `MemberLedgerId` and `AssignedAt`, sets `Status = Vacant`, and records `VacatedAt`. `Locked` is reserved but unused until the SRS defines it.

### AuditLog

- AuditLogId: Guid
- EntityType: string
- EntityId: Guid?
- ActionType: string
- PerformedByUserId: Guid
- OldStatus: string?
- NewStatus: string?
- ActionDescription: string?
- CreatedAt: DateTime

## Module 3 - Circle Marketplace and Membership Applications

### MembershipApplication

- MembershipApplicationId: Guid
- UserId: Guid?
- ListingId: Guid
- Name: string
- Email: string
- Phone: string
- NationalId: string
- Stage: MembershipApplicationStage
- CreatedAt: DateTime
- UpdatedAt: DateTime?

Note: UserId is nullable because guest users can apply. The target circle is reached through MembershipApplication -> MarketplaceListing -> Circle.

## Module 4 - Verification Management

### VerificationRound

- VerificationRoundId: Guid
- CircleId: Guid
- ReviewedByUserId: Guid
- RoundName: string
- Format: VerificationFormat

### VerificationSchedule

- VerificationScheduleId: Guid
- ApplicationId: Guid
- VerificationRoundId: Guid
- Date: DateOnly
- Time: TimeOnly
- LocationLink: string?
- VideoLink: string?
- Status: ScheduleStatus

### VerificationCriterion

- VerificationCriterionId: Guid
- VerificationRoundId: Guid
- CriterionName: string
- Weight: decimal
- DisplayOrder: int
- IsActive: bool

### VerificationChecklistSubmission

- VerificationChecklistSubmissionId: Guid
- VerificationScheduleId: Guid
- SubmittedByUserId: Guid
- CompositeScore: decimal
- OverallComments: string?
- SubmittedAt: DateTime

### VerificationCriterionRating

- VerificationCriterionRatingId: Guid
- VerificationChecklistSubmissionId: Guid
- VerificationCriterionId: Guid
- Rating: int
- Comments: string?

## Module 5 - Agreement and Payment Management

### MembershipAgreement

- MembershipAgreementId: Guid
- MembershipApplicationId: Guid
- MemberName: string
- CircleTitle: string
- ContributionSchedule: string
- PayoutSlot: int
- StartDate: DateOnly
- ExpiryDate: DateOnly
- Status: AgreementStatus
- CreatedAt: DateTime
- RespondedAt: DateTime?

### PaymentTransaction

- PaymentTransactionId: Guid
- MemberLedgerId: Guid
- CircleId: Guid
- RecordedByUserId: Guid
- TransactionType: PaymentTransactionType
- Amount: decimal
- PaymentMethod: PaymentMethod
- TransactionReference: string
- TransactionStatus: PaymentTransactionStatus
- ReceiptNumber: string
- ReceiptFilePath: string
- TransactionDate: DateTime
- CreatedAt: DateTime

Payment transactions belong to the active MemberLedger, not directly to the agreement.

## Module 6 - Onboarding and Member Ledger Activation

### OnboardingCase

- OnboardingCaseId: Guid
- MembershipAgreementId: Guid
- OrganizerId: Guid
- FinalStatus: OnboardingCaseStatus
- CreatedAt: DateTime

### DocumentRequirement

- DocumentRequirementId: Guid
- DocumentName: string
- Description: string?
- IsRequired: bool
- IsActive: bool
- DisplayOrder: int

### Document

- DocumentId: Guid
- OnboardingCaseId: Guid
- DocumentRequirementId: Guid
- ReviewedByUserId: Guid?
- FileName: string
- FilePath: string
- FileSize: long
- UploadedAt: DateTime
- Status: DocumentStatus
- ReviewedAt: DateTime?
- RejectionReason: string?

### MemberLedger

- MemberLedgerId: Guid
- UserId: Guid
- OnboardingCaseId: Guid
- ActivatedByAdminId: Guid
- ActivatedAt: DateTime

## MVP Status Values

Implementation note: these values are represented as C# enums in Domain. EF Core should store them as strings in SQL Server during Phase 1 mapping.

### CircleType

- NewCircle
- Replacement

### CircleRequestStatus

- Draft
- Submitted
- ModificationRequested
- Approved
- Rejected
- Published
- Cancelled

### CircleStatus

- Open
- InRecruitment
- Filled
- Closed

### MarketplaceListingStatus

- Active
- Completed
- Cancelled

### CircleSlotStatus

- Vacant
- Assigned
- Locked

### MembershipApplicationStage

- Submitted
- Shortlisted
- VerificationScheduled
- VerificationCompleted
- AgreementExtended
- Confirmed
- Rejected

### VerificationFormat

- InPerson
- Video
- Phone

### AgreementStatus

- Pending
- Accepted
- Declined
- Expired

### TransactionType

- PayIn
- PayOut

### PaymentMethod

- BankTransfer
- EWallet
