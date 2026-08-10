# SRS Relationships - Aman Money Loop MVP

This file reflects the current Domain model plus the agreed corrections needed for the MVP.

## Module 1 - User and Account Management

- Module 1 implementation is on hold until ASP.NET Core Identity is finalized.
- Admin-created users should be represented by a custom field on the Identity user, such as CreatedByAdminId.
- User to Role should use ASP.NET Core Identity role mapping.
- User to OtpToken may be custom if the team keeps a separate OTP table. OtpToken.UserId should be nullable to support pre-registration OTPs by email.
- Business entities keep user-related Guid FK fields for now. User navigation properties are commented in code until the final Identity user type is introduced.

## Module 2 - Circle Request and Configuration Management

- User(Circle Organizer) to CircleRequest: one-to-many through CircleRequest.CreatedByOrganizerId.
- User(Admin) to CircleRequest: one-to-many through CircleRequest.ReviewedByAdminId.
- CircleRequest to Circle: one-to-one for approved new circle requests through Circle.RequestId.
- Circle to CircleRequest replacement requests: one-to-many through CircleRequest.ExistingCircleId.
- Circle to MarketplaceListing: one-to-one for the MVP.
- Circle to CircleSlot: one-to-many.
- CircleSlot to MemberLedger: optional one-to-one while a slot is assigned.
- User to AuditLog: one-to-many through AuditLog.PerformedByUserId.

## Module 3 - Circle Marketplace and Membership Applications

- MarketplaceListing to MembershipApplication: one-to-many.
- Circle to MembershipApplication: no direct relationship. Applications reach the circle through MarketplaceListing.
- User(Member) to MembershipApplication: optional one-to-many. UserId is nullable because guests can apply.
- User(Organizer) to MarketplaceListing: managed indirectly through the listing's Circle and CircleRequest.CreatedByOrganizerId.

## Module 4 - Verification Management

- Circle to VerificationRound: one-to-many.
- User(Reviewer/Organizer) to VerificationRound: one-to-many through VerificationRound.ReviewedByUserId.
- VerificationRound to VerificationCriterion: one-to-many.
- VerificationRound to VerificationSchedule: one-to-many.
- MembershipApplication to VerificationSchedule: one-to-many.
- VerificationSchedule to VerificationChecklistSubmission: one-to-many.
- User(Reviewer/Organizer) to VerificationChecklistSubmission: one-to-many through SubmittedByUserId.
- VerificationChecklistSubmission to VerificationCriterionRating: one-to-many.
- VerificationCriterion to VerificationCriterionRating: one-to-many.

Correction from the earlier relationship draft: MembershipApplication and VerificationSchedule should not be modeled as many-to-many. The VerificationSchedule entity already represents the scheduled application-round link.

## Module 5 - Agreement and Payment Management

- MembershipApplication to MembershipAgreement: one-to-one.
- MembershipAgreement to OnboardingCase: one-to-one after the member accepts.
- MemberLedger to PaymentTransaction: one-to-many.
- Circle to PaymentTransaction: one-to-many.
- User(Admin/Organizer) to PaymentTransaction: one-to-many through RecordedByUserId.

Correction from the earlier relationship draft: MembershipAgreement to PaymentTransaction is not one-to-one. Payments happen repeatedly after ledger activation, so PaymentTransaction belongs to MemberLedger.

## Module 6 - Onboarding and Member Ledger Activation

- MembershipAgreement to OnboardingCase: one-to-one.
- User(Organizer) to OnboardingCase: one-to-many through OrganizerId.
- OnboardingCase to Document: one-to-many.
- DocumentRequirement to Document: one-to-many.
- User(Reviewer/Organizer) to Document: optional one-to-many through ReviewedByUserId.
- OnboardingCase to MemberLedger: one-to-one.
- User(Member) to MemberLedger: one-to-many.
- User(Admin) to MemberLedger: one-to-many through ActivatedByAdminId.
- MemberLedger to CircleSlot: optional one-to-one when assigned to a slot.
