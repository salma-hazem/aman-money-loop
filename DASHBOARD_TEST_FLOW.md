# Aman Money Loop Demo Dashboard Test Flow

This flow tests the Member, Organizer, and Admin dashboards against the Development demo data.

## 1. Start the application

Stop any older API process first so the latest backend and demo-data fixes are loaded.

Backend terminal:

```powershell
cd "aman-money-loop-backend/MonyLoop.API"
dotnet run --launch-profile https
```

Frontend terminal:

```powershell
cd "aman-money-loop-frontend"
npm start
```

Open `http://localhost:4200/login`.

The frontend calls `https://localhost:7281`. If the browser rejects the development certificate, open `https://localhost:7281/swagger` once and accept/trust the certificate.

## 2. Demo accounts

All accounts use password `Demo123#`.

| Scenario | Email | Role |
|---|---|---|
| System administration | `mohamed.mohsenf23+admin@gmail.com` | Admin |
| Circle management | `mohamed.mohsenf23+organizer@gmail.com` | Organizer |
| Activated member | `mohamed.mohsenf23+member@gmail.com` | Member |
| Onboarding in progress | `mohamed.mohsenf23+onboarding@gmail.com` | Member |
| Agreement pending | `mohamed.mohsenf23+agreement@gmail.com` | Member |
| Ready for activation | `mohamed.mohsenf23+ready@gmail.com` | Member |

Log out before changing accounts. This prevents an old role or cached dashboard state from affecting the next test.

## 3. Read-only baseline tests

Complete this section before approving, publishing, verifying, or activating anything. Those actions intentionally change the expected counts.

### A. Active Member dashboard

Log in as `mohamed.mohsenf23+member@gmail.com`.

Expected at `/console`:

- Page title is **Member Dashboard**.
- Applications: **1**.
- Current Stage: **Confirmed**.
- Documents Required: **0**.
- Next Contribution: **EGP 2,500**.
- Current journey shows **Cairo Growth Circle**.
- Quick actions include Browse Circles, Track Applications, Manage Documents, and Payments & Receipts.

Open **My Applications**:

- One Cairo Growth Circle application appears with stage Confirmed.

Open **Upload Documents**:

- The National ID requirement is satisfied/approved.

Open **Payments & Receipts**:

- Total successful pay-ins: **EGP 7,500**.
- Three successful EGP 2,500 pay-ins are present.
- One failed EGP 2,500 pay-in is present.
- One pending EGP 17,500 payout is present.

### B. Onboarding Member dashboard

Log in as `mohamed.mohsenf23+onboarding@gmail.com`.

Expected at `/console`:

- Applications: **1**.
- Current Stage: **Confirmed**.
- Documents Required: **1**, because the uploaded document is still Pending.
- Next Contribution: **Not active**.
- **Manage Documents** is the primary next action.

Open **Upload Documents**:

- An uploaded National ID PDF appears with Pending status.
- No ledger or payment history is available yet.

### C. Agreement Member dashboard

Log in as `mohamed.mohsenf23+agreement@gmail.com`.

Expected at `/console`:

- Applications: **1**.
- Current Stage: **Agreement extended**.
- Documents Required shows unavailable until agreement acceptance/onboarding.
- Next Contribution: **Not active**.
- Track Applications leads to the pending application.

### D. Activation-ready Member dashboard

Log in as `mohamed.mohsenf23+ready@gmail.com`.

Expected at `/console`:

- Applications: **1**.
- Current Stage: **Confirmed**.
- Documents Required: **0**.
- Onboarding is Approved but no ledger exists yet.
- Next Contribution: **Not active** until Admin activation.

### E. Organizer dashboard

Log in as `mohamed.mohsenf23+organizer@gmail.com`.

Expected at `/console`:

- My Requests: **9**.
- Pending Approval: **2**.
- Needs Attention: **2**.
- Active Circles: **2**.
- Assigned Cases: **3**.
- Requests Requiring Attention contains:
  - **Home Upgrade Circle** — Changes requested / Edit request.
  - **Small Business Circle** — Ready to publish / Publish request.
- My Circle Overview contains four owned circles covering Open, InRecruitment, Filled, and Closed states.

Open **Circle Requests**:

- Status coverage includes Draft, Submitted, Modification Requested, Approved, Rejected, Published, and Fulfilled.

Open **Applicant Pipeline**, then the Cairo Growth Circle pipeline:

- Nine applications are available across Submitted, Shortlisted, Verification Scheduled, Verification Completed, Agreement Extended, Confirmed, and Rejected stages.

Open **Document Review**:

- One Pending document named `onboarding-national-id.pdf` appears.
- **View PDF** opens a real demo PDF.
- Do not click Verify or Reject until the mutation tests below.

### F. Admin dashboard

Log in as `mohamed.mohsenf23+admin@gmail.com`.

Expected at `/console`:

- Approval Queue: **2**.
- Open Circles: **2** (one Open and one In Recruitment).
- Ready to Activate: **1**.
- Active Ledgers: **1**.
- Requests Requiring Action contains **Education Fund Circle** and **Wedding Plan Circle**.
- Circle Status snapshot currently contains:
  - Open: **1**
  - In Recruitment: **1**
  - Filled: **1**
  - Closed: **2**
  - Available slots: **18**

The second Closed circle is older data already present in the Development database, not a duplicate from the demo seeder.

Open **Ledger Activation**:

- One Approved onboarding case appears for Salma Ready.
- Do not activate it until the mutation tests.

Open **Approval Queue**:

- Both submitted requests can be opened.
- The detail screen provides Approve, Request Modification, Reject, and Audit Trail sections.

## 4. Role and route protection

While logged in as the Active Member, manually visit:

- `/console/admin/circle-requests`
- `/console/circle-requests`
- `/console/onboarding/activation`

Each unauthorized route should redirect back to `/console`.

While logged in as Organizer, visit `/console/admin/users`. It should also redirect to `/console`.

Log out and visit `/console`. It should redirect to `/login`.

## 5. Cross-dashboard mutation flow

This section changes the database. Perform it in the listed order.

### Flow 1: Document review to ledger activation

1. Log in as Organizer.
2. Open **Document Review**.
3. Open `onboarding-national-id.pdf` and confirm the demo PDF renders.
4. Click **Verify**.
5. Confirm the document disappears from the Pending review list.
6. Log in as the Onboarding Member.
7. Confirm Documents Required changed from **1** to **0**.
8. Log in as Admin.
9. Confirm Ready to Activate changed from **1** to **2**: Salma Ready plus Nour Onboarding.
10. Open **Ledger Activation** and activate both cases one at a time.
11. Confirm each row disappears and a success message appears.
12. Return to Admin Dashboard and refresh.

Expected after both activations:

- Ready to Activate: **0**.
- Active Ledgers: **3**.
- Cairo Growth Circle filled slots increase from **1** to **3**.
- Mona, Nour, and Salma occupy payout slots 1, 3, and 5 respectively.
- Nour and Salma Member dashboards now show an active ledger and EGP 2,500 next contribution.

### Flow 2: Admin approval to Organizer publication

1. Log in as Admin.
2. Open **Approval Queue** and select **Education Fund Circle**.
3. Click **Approve Request**.
4. Confirm it leaves the queue; queue count becomes **1**.
5. Log in as Organizer.
6. Confirm Education Fund Circle now appears as **Ready to publish**.
7. Open it and click **Publish to Marketplace**.
8. Confirm its status changes to Published and it appears in Circle Registry as In Recruitment.
9. Log in as a Member and open **Marketplace**.
10. Confirm Education Fund Circle is visible.

Optional final application test:

1. Stay logged in as `mohamed.mohsenf23+agreement@gmail.com`.
2. Apply to Education Fund Circle.
3. Confirm My Applications increases from **1** to **2**.
4. Log in as Organizer and open that circle's pipeline.
5. Confirm the new application appears as Submitted.
6. Check `mohamed.mohsenf23@gmail.com`; SMTP messages for the scenario should retain the `+agreement` recipient alias.

### Flow 3: Admin decision alternatives

Use **Wedding Plan Circle** to test one alternative decision:

- **Request Modification**: provide a reason and confirm the Organizer sees it under Needs Attention with an Edit action.
- Or **Reject**: provide a reason and confirm it leaves the Admin queue and appears Rejected for the Organizer.

Choose only one because both are terminal decisions for the same submitted request state.

## 6. Pass criteria

The dashboard test passes when:

- Each role sees only its permitted navigation and dashboard.
- Baseline cards match the expected values before mutations.
- Every dashboard card and quick action opens the correct page.
- Loading, empty, success, and error messages do not overlap or remain stuck.
- Document review updates the Member and Admin dashboards after refresh.
- Activation updates ledger counts, circle capacity, and the activated Member dashboard.
- Approval and publication update Admin, Organizer, and Member marketplace views.
- English and Arabic layouts remain readable at desktop and mobile widths.
- Browser console contains no uncaught errors, and dashboard API calls return 2xx responses.
