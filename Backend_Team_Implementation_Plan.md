# Aman Money Loop Backend Team Implementation Plan

## 1. Purpose

This plan organizes the remaining backend work for five team members using the existing Onion Architecture solution:

- `Mony_Loop.Domain`: entities, enums, domain rules, and domain-level contracts.
- `Mony_Loop.Application`: use cases, DTOs, validation, service abstractions, mapping, and result models.
- `Mony_Loop.Infrastructure`: EF Core, ASP.NET Core Identity, repositories, email, file storage, and other external services.
- `Mony_Loop.API`: controllers, authentication setup, middleware, dependency injection, and Swagger.

Every member owns one business module from Domain through API. Module 1 is shared because authentication, users, roles, and authorization are required by every other module.

## 2. Current Repository Status

### Already present

- The four Onion Architecture projects exist and their project-reference direction is generally correct.
- Domain entities and enums exist for Modules 2-6.
- The main entity relationships are documented in `SRS entities.md` and `SRS relationships.md`.
- Status and type values are C# enums.
- EF Core enum conversions store configured enums as strings.
- Module 2 EF configurations exist for:
  - `CircleRequest`
  - `Circle`
  - `MarketplaceListing`
  - `CircleSlot`
  - `AuditLog`
- Module 5 EF configurations exist for:
  - `MembershipAgreement`
  - `PaymentTransaction`
- `MonyLoopDbContext` currently exposes the Module 2 and Module 5 entities.

### Still missing

- Module 1 ASP.NET Core Identity implementation.
- Identity database mapping and relationships to business user ID fields.
- EF configurations and `DbSet` properties for Modules 3, 4, and 6.
- A final reviewed initial migration.
- Database connection and `DbContext` registration in the API.
- Application DTOs, validators, interfaces, services/use cases, and mappings.
- Repository or data-access abstractions and implementations.
- REST API controllers for all modules.
- JWT authentication and role/policy authorization.
- Email/OTP delivery, notification abstraction, and templates.
- Secure file storage and validation for onboarding documents and receipts.
- Global exception handling, completed `Result`/`Error` models, API error responses, and request validation.
- Unit and integration test projects.
- Removal or replacement of placeholder files such as `Class1.cs` and `WeatherForecastController.cs`.
- Security and package review, including the current AutoMapper vulnerability warning.

### Important migration warning

Do not create the final initial migration while only some entity configurations are present. EF Core can discover related entities through navigation properties and map unfinished modules by convention. Generate the initial migration only after all entity configurations and Identity mapping are merged and reviewed.

### SRS decisions that are still open

These points must be agreed in Phase 0 because the original SRS wording and the current Domain model are not fully identical:

- The original request story says a submitted request becomes `Pending`, while the current enum uses `Submitted`. Choose one term; `Submitted` is currently the code/documentation standard.
- The original agreement precondition says `Verification Completed - Selected`, while the current application enum only has `VerificationCompleted`. Decide whether selection is a separate stage/decision or whether completion itself means selected.
- The original onboarding story says the circle request becomes `Fulfilled` after ledger activation. With several circle slots, clarify whether this happens after the first activation or only after every approved slot is assigned. The safer rule is to mark it fulfilled when the circle is filled.
- Module 5 contains `PaymentTransaction`, but the reduced SRS does not provide a detailed payment user story. Confirm whether payment create/list/receipt endpoints are MVP deliverables or only the data model is required.
- Confirm whether approving a new circle request automatically creates its listing or whether the Organizer must explicitly publish it, as the story wording suggests.
- Confirm whether replacement requests create a new circle, modify the existing circle, or only fill one vacant slot. This determines the approval transaction and status changes.

## 3. Team Ownership

Replace `Member 1` through `Member 5` with actual names before starting implementation.

| Owner | Primary module | User stories | Main ownership |
|---|---|---|---|
| Member 1 | Module 2 - Circle Request and Configuration | US4-US5 | Requests, approval, circles, listings, slots, audit records |
| Member 2 | Module 3 - Marketplace and Applications | US6-US7 | Marketplace browsing, guest/member applications, shortlist/reject |
| Member 3 | Module 4 - Verification | US8-US9 | Rounds, schedules, criteria, checklist scoring |
| Member 4 | Module 5 - Agreement and Payment | US10-US11 plus payment records | Agreement lifecycle first; payments after ledger integration |
| Member 5 | Module 6 - Onboarding and Member Ledger | US12 | Documents, review, activation, ledger and slot assignment |
| All members | Module 1 - User and Account Management | US1-US3 | Identity, registration, login, password reset, roles and authorization |

Each primary owner implements and tests all layers for that module. Ownership means that other members do not directly redesign or edit that module without coordinating with its owner.

## 4. Shared Module 1 Split

All members participate in Module 1, but each shared area has one owner to prevent merge conflicts.

| Member | Module 1 responsibility | Deliverables |
|---|---|---|
| Member 1 | Identity data foundation | `ApplicationUser`, Identity role type, Identity `DbContext` integration, Identity options, user-related EF relationships |
| Member 2 | Registration and email confirmation | Register DTO/validator/service/endpoint, duplicate email checks, OTP/email-confirmation flow |
| Member 3 | Login and JWT | Login DTO/service/endpoint, JWT generation, claims, authenticated-current-user abstraction |
| Member 4 | Forgot/reset password | Request-reset and confirm-reset DTOs/services/endpoints, secure token or OTP validation, expiry and one-time use |
| Member 5 | Roles, authorization, and Identity testing | Admin/Organizer/Member role seeding, authorization policies, access tests, Swagger bearer setup |

### Module 1 decisions required before coding

The team must agree on these contracts together:

1. Use ASP.NET Core Identity with `Guid` keys.
2. Keep `ApplicationUser` in Infrastructure because it depends on Identity classes.
3. Keep business entities dependent only on user IDs; do not add Infrastructure Identity types to Domain.
4. Expose Identity behavior to Application through interfaces such as `IIdentityService`, `ITokenService`, and `ICurrentUserService`.
5. Use roles `Admin`, `Organizer`, and `Member` with one agreed spelling and casing.
6. Decide whether email confirmation and password reset use Identity's built-in tokens or a custom `OtpToken` table.
7. Decide JWT lifetime, issuer, audience, signing-key storage, and whether refresh tokens are in MVP scope.
8. Decide who can create Organizer and Admin accounts. Public registration should create Members only unless the SRS is changed.

### Recommended OTP decision

Use a custom OTP record only if the product must accept a visible six-digit code exactly as described by the SRS. Use Identity's token providers for the actual password-reset authorization where possible. Never store a plain OTP; store a hash, purpose, expiry, used time, and attempt count.

### Module 1 completion gate

Module 1 is complete when:

- A member can register and confirm their email/OTP.
- A registered user can log in and receive a JWT.
- A user can complete the forgot-password flow.
- Roles are seeded and protected endpoints enforce them.
- Other modules can read the authenticated user's ID and role through Application abstractions.
- Identity integration tests pass.

## 5. Dependency Map

### End-to-end business flow

```text
Module 1 Identity
       |
       v
Module 2 Circle Request -> Circle -> Marketplace Listing
       |                              |
       |                              v
       |                    Module 3 Membership Application
       |                              |
       |                              v
       |                    Module 4 Verification
       |                              |
       |                              v
       |                    Module 5 Membership Agreement
       |                              |
       |                              v
       +-------------------- Module 6 Onboarding + Member Ledger
                                      |
                                      v
                            Module 5 Payment Transactions
```

### Cross-module dependency table

| Consumer work | Required provider contract/data | Hard dependency point |
|---|---|---|
| Every protected module | Module 1 current user and role authorization | Endpoint integration and authorization tests |
| Module 2 request creation/review | Module 1 Organizer/Admin IDs | Real authenticated workflows |
| Module 3 marketplace browse | Module 2 active listing and circle data | Query implementation |
| Module 3 application submission | Module 2 active listing validation; optional Module 1 user | Submission validation |
| Module 4 schedule verification | Module 3 shortlisted application; Module 2 verification round/circle | Schedule command |
| Module 4 complete verification | Module 3 stage update | Final checklist completion |
| Module 5 generate agreement | Module 3 application; Module 4 verification completed | Agreement creation |
| Module 6 create onboarding case | Module 5 accepted agreement | Onboarding start |
| Module 6 activate ledger | Module 1 member; Module 2 vacant slot/circle/request | Activation transaction |
| Module 5 record payment | Module 6 active ledger; Module 2 circle | Payment command |

### How parallel work remains possible

A consumer module does not need to wait for the full provider implementation if the provider first publishes a small Application contract. For example, Module 4 can code against an agreed `IMembershipApplicationGateway` that can verify a shortlisted application and update its stage. During development it can be mocked; the real Module 3 implementation is connected during integration.

Do not access another module's repository or `DbContext` directly from a service. Call an Application abstraction owned and documented by the provider module. This keeps module dependencies explicit and testable.

## 6. Phased Execution Plan

## Phase 0 - Baseline and Team Agreement

Goal: freeze the contracts and conventions that everyone will use.

### Tasks

- Review `SRS entities.md`, `SRS relationships.md`, and the original MVP SRS.
- Confirm the ownership table and replace member placeholders with names.
- Confirm endpoint naming, DTO naming, namespace naming, and folder structure.
- Confirm enum values and valid status transitions.
- Agree on validation rules, pagination shape, date/time handling, and API error format.
- Agree on delete behavior and whether records are deleted, archived, or status-controlled.
- Resolve current naming inconsistencies such as namespaces containing `___` before parallel work expands them.
- Decide the Module 1 OTP/token approach.
- Decide the cross-cutting interfaces listed in Phase 1.
- Create a short API contract per module before implementing controllers.

### Output

- Approved SRS entity and relationship documents.
- Approved status-transition table.
- Approved API conventions.
- Named module owners.
- No unresolved relationship decision that would change a database key.

### Gate

Do not start migrations or broad service implementation until entity relationships and Identity key type are approved.

## Phase 1 - Shared Architecture Foundation

Goal: provide one stable pattern that every module follows.

### Shared tasks

- Complete `Result` and `Error` with one consistent success/failure model.
- Add global exception handling and standardized API errors.
- Add request validation, using one agreed library or manual validation pattern.
- Define pagination request/result types.
- Define `ICurrentUserService`.
- Define `IIdentityService` and `ITokenService`.
- Define `IEmailSender` and a notification abstraction.
- Define `IFileStorageService` for documents and receipts.
- Decide the data-access pattern: focused repositories or Application query/command interfaces.
- Add Application and Infrastructure dependency-injection extension methods.
- Register `DbContext` and SQL Server connection configuration.
- Configure Swagger basics.
- Create unit-test and API integration-test projects.

### Ownership

- Member 1: database registration and data-access pattern example.
- Member 2: validation and mapping example.
- Member 3: `Result`, `Error`, exception handling, and API response example.
- Member 4: email/file-storage interfaces and development implementations.
- Member 5: test projects, test fixtures, and Swagger conventions.

### Output

One reference vertical slice should compile and demonstrate Domain -> Application -> Infrastructure -> API without implementing a full business module.

### Gate

Each member must use the shared reference pattern. Changes to common contracts require team review.

## Phase 2 - Module 1 Identity Foundation

Goal: make authentication and authorization available to all business modules.

### Work sequence

1. Member 1 implements Identity entities, `Guid` keys, context inheritance/integration, and options.
2. Member 5 adds role seed data and authorization policies after the role model exists.
3. Members 2, 3, and 4 implement registration, login/JWT, and password reset against the agreed Identity abstractions.
4. Member 5 adds integration tests and Swagger bearer authentication.
5. Member 1 maps business user-ID relationships only after `ApplicationUser` is stable.

### Integration rule

Only Member 1 edits the Identity context and core Identity registration while this phase is active. Other members work through interfaces to avoid repeated changes to the same files.

### Gate

Merge Module 1 before connecting real user navigation/FK behavior in other modules. Other module business logic may proceed using fake `ICurrentUserService` implementations in tests.

## Phase 3 - Module Data Mapping in Parallel

Goal: finish and review the complete EF model before the initial migration.

### Member 1 - Module 2

- Re-review existing configurations against final Identity and Module 6 relationships.
- Add Organizer/Admin/AuditLog Identity relationships if the team chooses database FKs.
- Verify request-to-circle and replacement-request constraints.
- Verify listing and slot uniqueness rules.

### Member 2 - Module 3

- Configure `MembershipApplication`.
- Configure optional `UserId` and required `ListingId`.
- Configure the one-to-many listing/application relationship.
- Add indexes for listing, user, email, and stage as justified by queries.

### Member 3 - Module 4

- Configure rounds, schedules, criteria, submissions, and ratings.
- Store all enums as strings with the agreed maximum length.
- Add uniqueness to prevent duplicate criterion ratings in one submission.
- Add indexes for application schedules, reviewer queues, round order, and status.

### Member 4 - Module 5

- Re-review the existing agreement and payment configurations.
- Complete cross-module FKs after Modules 3 and 6 mappings are available.
- Confirm receipt/reference nullability and indexes used by payment lookup.

### Member 5 - Module 6

- Configure onboarding cases, requirements, documents, and ledgers.
- Enforce one agreement -> one onboarding case.
- Enforce one onboarding case -> one member ledger.
- Configure document review and active requirement indexes.
- Complete the optional one-to-one ledger/slot relationship with Member 1.

### Shared database integration

- One nominated migration owner merges configuration PRs in dependency order.
- The migration owner adds all final `DbSet` properties.
- The team reviews EF's generated model and migration together.
- Check that every enum column is a string and every one-to-one relationship has a unique index.
- Check cascade paths explicitly; prefer `Restrict` where deleting one workflow record must not erase history.
- Generate one clean initial migration only after all mappings pass review.

### Gate

The database can be created from an empty SQL Server instance, all expected tables/constraints exist, and a basic persistence test passes for every module.

## Phase 4 - Independent Module Vertical Slices

Goal: implement each module from Application through API in parallel.

Each owner should build one use case at a time in this order:

1. Request/response DTOs.
2. Validation.
3. Application interface and service/use case.
4. Infrastructure query/repository implementation.
5. Controller endpoint and authorization.
6. Unit tests.
7. API integration test.
8. Swagger example and error cases.

### Member 1 - Module 2 backlog

- Create circle request as Draft.
- Edit own Draft or ModificationRequested request.
- Submit request after completeness validation.
- List organizer's requests.
- List Admin approval queue.
- Admin requests modification, approves, or rejects with reason.
- Create the approved circle and slots safely and idempotently.
- Publish/cancel the marketplace listing according to the approved workflow.
- Record important transitions in `AuditLog`.

### Member 2 - Module 3 backlog

- List and paginate active marketplace listings.
- View listing/circle details.
- Submit an application as a guest or registered member.
- Prevent invalid application to inactive/completed listings.
- List applications for the correct Organizer's circle.
- Shortlist or reject an application with transition validation.
- Send confirmation and decision notifications.

### Member 3 - Module 4 backlog

- Create/manage verification rounds and criteria for a circle.
- Schedule a shortlisted application for a round.
- Validate future date/time and format-specific links/location.
- List reviewer and member schedules.
- Submit one complete rating set for a checklist.
- Validate ratings are 1-5 and every active criterion is rated once.
- Calculate the weighted composite score.
- Mark the application VerificationCompleted only when all required rounds are complete.

### Member 4 - Module 5 backlog

- Generate an agreement only for an eligible verified application.
- Validate payout slot, start date, and future expiry.
- Retrieve an agreement through a secure member response flow.
- Accept, decline, or expire an agreement exactly once.
- Trigger onboarding creation through a Module 6 contract after acceptance.
- Record pay-in/pay-out transactions only for active ledgers.
- Upload/reference optional receipts securely.
- List and filter ledger/circle payment history with authorization.

Member 4 should implement agreements before payments. Payment integration waits for Module 6 ledger activation.

### Member 5 - Module 6 backlog

- Create one onboarding case for an accepted agreement.
- List active document requirements.
- Upload PDF documents under the configured size limit.
- Store file metadata separately from file content.
- Review, verify, or reject documents with a reason.
- Allow activation only when every required document is verified.
- In one database transaction, create the member ledger, assign the reserved/vacant slot, update counts/statuses, and mark the request fulfilled when appropriate.
- Prevent duplicate ledger creation and duplicate slot assignment.

### Gate

Each module passes its own unit and API integration tests using mocks/fakes for provider modules where real integrations are not yet merged.

## Phase 5 - Ordered Cross-Module Integration

Goal: replace mocks with real provider implementations and prove the complete lifecycle.

Integrate in this exact order:

1. Module 1 -> Module 2: authenticated Organizer creates; Admin reviews.
2. Module 2 -> Module 3: approved/published listing accepts applications.
3. Module 3 -> Module 4: shortlisted application is scheduled and verified.
4. Module 4 -> Module 5 agreement: verified application receives an agreement.
5. Module 5 agreement -> Module 6: accepted agreement creates onboarding.
6. Module 6 -> Module 2: activation assigns a slot and updates circle/request state.
7. Module 6 -> Module 5 payments: active ledger permits payment transactions.

### Integration responsibilities

The consumer owner writes the consumer-side integration and tests. The provider owner supplies the interface implementation and reviews that its module invariants are not bypassed.

### Required end-to-end scenario

```text
Register/Login
-> Create and approve circle request
-> Publish listing
-> Submit and shortlist application
-> Schedule and complete verification
-> Generate and accept agreement
-> Upload and verify documents
-> Activate ledger and assign slot
-> Record payment
```

### Gate

The full scenario passes automatically against a real test database, including authorization failures and invalid status transitions.

## Phase 6 - Hardening and Delivery

Goal: make the backend reliable and ready for demonstration/deployment.

### Security

- Verify every endpoint has intentional anonymous/role/policy access.
- Do not expose password hashes, OTPs, tokens, file paths, or internal exceptions.
- Add rate limiting to login, OTP, password reset, and public application endpoints.
- Validate file type by content where practical, not extension alone.
- Move secrets and connection strings out of committed settings.
- Review NuGet vulnerability warnings and update or replace affected packages.

### Reliability and data integrity

- Add optimistic concurrency where two reviewers/admins may update the same record.
- Use database transactions for multi-entity state changes.
- Make approval, agreement acceptance, onboarding activation, and payment recording idempotent.
- Add cancellation-token support to async operations.
- Add structured logging without personal or financial data leakage.
- Verify indexes against actual list/filter endpoints.

### Testing

- Domain/unit tests for validation, calculations, and state transitions.
- Application tests for every use case and authorization decision.
- Infrastructure tests for EF constraints and external-service adapters.
- API integration tests for happy paths, invalid input, forbidden access, conflicts, and not-found responses.
- One complete end-to-end lifecycle test.

### Cleanup

- Replace or remove `Class1.cs`.
- Replace or remove empty `Result`, `Error`, and `ApiBaseController` placeholders.
- Remove `WeatherForecastController` and unused sample request files.
- Remove unused packages and stale folders.
- Normalize namespaces and spelling without changing behavior.
- Update `README.md` with setup, migration, seed-user, and run instructions.

### Final gate

- Clean build with no unresolved warnings.
- All automated tests pass.
- Database can be recreated from migrations.
- Swagger documents the complete API and authorization requirements.
- A new developer can run the project from the README.

## 7. Coordination Rules for Parallel Work

### File ownership

- One owner at a time for `MonyLoopDbContext.cs`, `Program.cs`, DI extension files, shared result/error types, and migrations.
- Module owners work mainly inside module-specific folders in every project.
- Common contract changes require a small reviewed PR before consumer modules adopt them.

### Branch and PR approach

- Use short-lived branches such as `feature/module-3-submit-application`.
- Keep one use case or closely related vertical slice per PR.
- Rebase/update from the shared integration branch before requesting review.
- Require at least one review from a module affected by a cross-module contract.
- Do not combine generated migrations from several parallel branches.
- Merge provider contracts before consumer implementations that depend on them.

### Contract-first handoff

For every cross-module dependency, the two owners agree on:

- Method name and purpose.
- Input and output DTOs.
- Possible errors.
- Required status before the call.
- State transition caused by the call.
- Transaction ownership.
- Idempotency behavior.

The module that owns the business invariant owns the state change. For example, Module 4 may decide verification is complete, but it asks Module 3 to perform the valid application-stage transition rather than directly editing Module 3 data.

### Daily synchronization

Each member reports:

- Completed use case/PR.
- Contract changed or needed.
- Current dependency/blocker.
- Next integration point.

Focus synchronization on contracts and blockers, not line-by-line implementation.

## 8. Recommended Cross-Module Contracts

These are planning-level names; finalize signatures during Phase 1.

| Provider | Example Application abstraction | Consumers |
|---|---|---|
| Module 1 | `ICurrentUserService`, `IIdentityService` | All modules |
| Module 2 | `ICircleReadService`, `IListingAvailabilityService`, `ISlotAssignmentService` | Modules 3, 4, 5, 6 |
| Module 3 | `IMembershipApplicationWorkflow` | Modules 4 and 5 |
| Module 4 | `IVerificationEligibilityService` | Module 5 |
| Module 5 | `IAgreementWorkflow` | Module 6 |
| Module 6 | `IMemberLedgerReadService`, `IOnboardingWorkflow` | Module 5 and Module 2 integration |
| Shared Infrastructure | `IEmailSender`, `IFileStorageService`, `IAuditService` | Relevant modules |

Avoid a single large service that exposes all module data. Small purpose-specific contracts reduce coupling and make mocking simpler.

## 9. Definition of Done for Every Module

A module is not complete when only its entities or controllers exist. It is complete when:

- Domain model and status transitions match the approved SRS.
- EF configuration, keys, relationships, indexes, enum conversions, and delete behavior are reviewed.
- DTOs do not expose EF entities directly.
- Validation covers required fields and business rules.
- Services enforce authorization and valid state transitions.
- External dependencies are behind Application abstractions.
- Controllers use correct HTTP methods and status codes.
- Unit tests cover business rules.
- Integration tests cover persistence, authorization, and endpoint behavior.
- Swagger accurately describes requests, responses, and authorization.
- No secret or sensitive data is logged or returned.
- The module owner has documented any contract used by another module.

## 10. Immediate Next Actions

1. Replace the five member placeholders with real names and confirm module assignment.
2. Finish Phase 0 decisions, especially Identity `Guid` keys, OTP approach, state transitions, and namespace cleanup.
3. Implement the shared Phase 1 reference pattern.
4. Implement Module 1 using the five-way split above.
5. In parallel, allow module owners to finish their EF configuration and prepare DTO/API contracts, but do not generate the final migration yet.
6. Assign one migration owner and one temporary integration owner for shared API/DI files.
7. Start vertical slices after the Identity and common Application contracts stabilize.
