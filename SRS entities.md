# SRS Entities - Aman Money Loop MVP

This file reflects the current Domain model plus the agreed corrections needed for the MVP Onion Architecture implementation.

## Module 1 - User and Account Management

Implementation note: Module 1 authentication is on hold until the team decides the ASP.NET Core Identity setup. The current code does not contain custom User/Role/UserRole entities. Business entities keep user-related Guid FK fields for now, and user navigation properties are commented until the Identity user type is finalized.

### ApplicationUser

- UserId: Guid
- FirstName: string
- LastName: string
- Email: string
- PasswordHash: string
- PhoneNumber: string
- NationalId: string?
- MustChangePassword: bool
- CreatedByAdminId: Guid?
- CreatedAt: DateTime

Note: Intended to be implemented later as a customized ASP.NET Core Identity user, not a fully custom authentication entity.

### ApplicationRole

- RoleId: Guid
- RoleName: string

Note: Intended to use ASP.NET Core Identity roles/user-role mapping.

### UserRole

- UserRoleId: Guid
- UserId: Guid
- RoleId: Guid

### OtpToken

- OtpTokenId: Guid
- UserId: Guid?
- Email: string
- Code: string
- Purpose: string
- IsUsed: bool
- ExpiresAt: DateTime
- CreatedAt: DateTime
- UsedAt: DateTime?

Note: OTP may be custom because the SRS needs email OTP confirmation and password reset flows. Final implementation can either use Identity tokens directly or keep a separate OtpToken table.

## Module 2 - Circle Request and Configuration Management

### CircleRequest

- RequestId: Guid
- ExistingCircleId: Guid?
- CreatedByOrganizerId: Guid
- ReviewedByAdminId: Guid?
- CircleTitle: string
- CircleType: string
- ContributionAmount: decimal
- Duration: int
- NumberOfSlots: int
- ShortJustification: string?
- RequestStatus: string
- VacantSlotNumber: int?
- CreatedAt: DateTime
- SubmittedAt: DateTime?
- ReviewedAt: DateTime?
- DecisionReason: string?

### Circle

- CircleId: Guid
- RequestId: Guid
- ApprovedSlots: int
- FilledCount: int
- Amount: decimal
- Duration: int
- Status: string

### MarketplaceListing

- ListingId: Guid
- CircleId: Guid
- ListingStatus: string

### CircleSlot

- CircleSlotId: Guid
- CircleId: Guid
- MemberLedgerId: Guid?
- SlotNumber: int
- Status: string
- VacatedAt: DateTime?
- AssignedAt: DateTime?

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
- Stage: string
- CreatedAt: DateTime
- UpdatedAt: DateTime?

Note: UserId is nullable because guest users can apply. The target circle is reached through MembershipApplication -> MarketplaceListing -> Circle.

## Module 4 - Verification Management

### VerificationRound

- VerificationRoundId: Guid
- CircleId: Guid
- ReviewedByUserId: Guid
- RoundName: string
- Format: string

### VerificationSchedule

- VerificationScheduleId: Guid
- ApplicationId: Guid
- VerificationRoundId: Guid
- Date: DateOnly
- Time: TimeOnly
- LocationLink: string?
- VideoLink: string?
- Status: string

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
- Status: string
- CreatedAt: DateTime
- RespondedAt: DateTime?

### PaymentTransaction

- PaymentTransactionId: Guid
- MemberLedgerId: Guid
- CircleId: Guid
- RecordedByUserId: Guid
- TransactionType: string
- Amount: decimal
- PaymentMethod: string
- TransactionReference: string
- TransactionStatus: string
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
- FinalStatus: string
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
- Status: string
- ReviewedAt: DateTime?
- RejectionReason: string?

### MemberLedger

- MemberLedgerId: Guid
- UserId: Guid
- OnboardingCaseId: Guid
- ActivatedByAdminId: Guid
- ActivatedAt: DateTime

## MVP Status Values

### CircleType

- NewCircle
- Replacement

### CircleRequestStatus

- Draft
- Submitted
- ModificationRequested
- Approved
- Rejected
- Cancelled
- Fulfilled

### CircleStatus

- Open
- InRecruitment
- Filled

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
