# SRS Relationships - Aman Money Loop MVP

This file reflects the current Domain model plus the agreed corrections needed for the MVP.

## Module 1 - User and Account Management

- ApplicationUser to ApplicationRole: many-to-many through ASP.NET Core Identity `UserRoles`.
- ApplicationUser(Admin) to ApplicationUser(created account): optional one-to-many through `RegisteredByAdminId`.
- ApplicationUser to OTPToken: one-to-many. `OTPToken.UserId` is required because the unconfirmed Identity user is created before the registration OTP is issued.
- ApplicationUser to RefreshToken: one-to-many.
- Canonical role names across every module are `Admin`, `Organizer`, and `Member`.

Lifecycle and security rules:

- Public Member registration uses a member-supplied password and requires a single-use registration OTP before login.
- Admin-created Organizer/Admin accounts are email-confirmed, receive a temporary password, and are blocked from protected workflows until changing it.
- Forgot Password starts password recovery by emailing an OTP; Reset Password completes that same flow with the OTP and new password.
- Authenticated Change Password requires the current password and does not use an OTP.
- Email changes remain pending until an OTP sent to the new address is confirmed.
- OTPs are stored as hashes in SQL, expire after ten minutes, allow five verification attempts, and are invalidated when used or replaced.
- Refresh tokens rotate on refresh and are revoked after security-sensitive account changes.

## Module 2 - Circle Request and Configuration Management

- User(Circle Organizer) to CircleRequest: one-to-many through CircleRequest.CreatedByOrganizerId.
- User(Admin) to CircleRequest: one-to-many through CircleRequest.ReviewedByAdminId.
- CircleRequest to Circle: one-to-one for approved new circle requests through Circle.RequestId.
- Circle to CircleRequest replacement requests: one-to-many through CircleRequest.ExistingCircleId.
- Circle to MarketplaceListing: one-to-one for the MVP.
- Circle to CircleSlot: one-to-many.
- CircleSlot to MemberLedger: optional one-to-one while a slot is assigned.
- User to AuditLog: one-to-many through AuditLog.PerformedByUserId.

Lifecycle rules:

- Approving a new-circle request creates exactly one Circle and its numbered CircleSlots; approving a replacement request creates neither.
- Publishing creates or reactivates the Circle's one MarketplaceListing.
- Only one active replacement request may target the same Circle and vacant slot number.
- Assigning a MemberLedger fills one CircleSlot; vacating it removes that optional one-to-one link.
- Filling the final slot completes the listing. Vacating a slot reopens the circle but does not automatically reactivate the listing.
- Cancelling an original published request closes its Circle; cancelling the last published replacement returns the existing Circle to `Open` and never closes it.

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
