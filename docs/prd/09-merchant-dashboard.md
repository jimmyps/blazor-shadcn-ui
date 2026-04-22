# PRD-09 Merchant Dashboard (Document B)

## 1. Overview
Merchant Dashboard is the operational console for merchant-side users to manage business settings, staff access, locations, and operational entry points for products, orders, kitchen operations, and reporting.

Primary users:
- Merchant Owner
- Manager
- Staff/Operator
- Optional Finance/Admin role (tenant-specific)
- Internal Support persona (restricted support tooling)

### MVP Scope (Document B)
MVP for Document B focuses on secure merchant/staff access and foundational dashboard operations:
1. Staff OTP login and authenticated session lifecycle.
2. Role/permission based access for Owner/Manager/Staff.
3. Tenant and location context selection and enforcement.
4. Core management pages (tenant, staff, location) and navigation entry points to Product, Order/KDS, and Reports modules.
5. Audit trail for sensitive administrative actions.

Cross-reference: PRD-00 Master Plan, PRD-01 Platform Backbone.

## 2. Goals & Success Criteria
1. **Fast secure access**: 95% of valid staff OTP login flows complete in <= 90 seconds end-to-end.
2. **Authorization correctness**: 100% of restricted endpoints enforce role/permission checks in integration tests and audit logs.
3. **Location-safe operations**: 100% of write actions include tenant + location scope validation; zero cross-location write leakage.
4. **Operational usability**: New merchant can complete onboarding baseline (tenant profile, first staff invite, first location setup) in <= 20 minutes median.
5. **Auditability**: 100% of sensitive actions generate immutable audit records with actor, tenant, location, action, timestamp, and target.

## 3. User Personas
### Merchant Owner
- Full control over tenant settings, billing profile, and permission policies.
- Requires visibility across all locations and management modules.

### Manager
- Oversees daily operations for one or more assigned locations.
- Manages staff assignments, operational settings, and routine reports.

### Staff/Operator
- Executes day-to-day workflows (order handling, KDS monitoring, basic lookup/report views as permitted).
- Limited configuration powers.

### Finance/Admin (Optional)
- Focuses on transaction reconciliation, refunds visibility, and ledger/report exports.
- Usually no product or staffing permission by default.

### Internal Support Persona
- Platform support role for diagnostics with explicit scoped impersonation/support tools.
- Cannot perform owner-only destructive actions.

## 4. Functional Requirements by Module

### 4.1 Authentication & Access
#### User Stories
**US-MD-001 Staff OTP Login**  
As a staff user, I can request and verify OTP to sign in without password.

Acceptance Criteria:
1. OTP request enforces throttle limits and abuse checks.
2. OTP verify returns access token + refresh token pair when valid.
3. Failed attempts are recorded in AuditLog.

**US-MD-002 Session Management**  
As an authenticated staff user, I can keep my session active securely via refresh rotation.

Acceptance Criteria:
1. Access token expiry is short-lived; refresh endpoint rotates refresh token.
2. Reused refresh token is rejected and flagged.
3. Logout revokes active session chain for current device.

**US-MD-003 Role/Permission Enforcement**  
As a tenant owner, I can ensure each role only sees/uses permitted features.

Acceptance Criteria:
1. Owner has full tenant permissions.
2. Manager and Staff permissions are constrained by role policy.
3. Unauthorized actions return 403 with auditable denial event.

**US-MD-004 Location Selection & Scope**  
As multi-location staff, I can select my active location and operate within that scope.

Acceptance Criteria:
1. Assigned locations are visible at login/start session.
2. Active location switch updates effective context for subsequent API calls.
3. Cross-location write attempts are rejected.

**US-MD-005 Sensitive Action Audit Trail**  
As owner/compliance reviewer, I can trace who changed privileged settings.

Acceptance Criteria:
1. Role changes, deactivation, location changes, and config updates emit AuditLog entries.
2. Audit entry includes actor, role, tenantId, locationId, target entity, before/after summary, and timestamp.
3. Audit list is filterable by date range, actor, and event type.

#### Business Rules
- **BR-MD-001** OTP is single-use and expires quickly.
- **BR-MD-002** Access token is JWT; refresh token rotation is mandatory.
- **BR-MD-003** Role grants baseline permissions; explicit overrides allowed only to authorized administrators.
- **BR-MD-004** Every protected request must carry tenant scope; staff operational requests also require location scope.
- **BR-MD-005** Sensitive actions must be written to immutable audit storage before success response.

### 4.2 Tenant Settings
#### User Stories
**US-MD-006** Owner updates business profile, branding, and operational defaults.  
Acceptance Criteria: profile updates validate required fields; branding changes are versioned; updates are auditable.

#### Business Rules
- **BR-MD-006** Tenant settings changes require Owner or delegated permission.
- **BR-MD-007** Business profile fields support draft-save and final publish states.

### 4.3 Staff Management
#### User Stories
**US-MD-007** Owner/Manager invites staff with role assignment and location binding.  
Acceptance Criteria: invite creates pending staff record; invite status trackable; assigned permissions generated.

**US-MD-008** Owner/Manager deactivates staff and updates role.  
Acceptance Criteria: deactivated staff sessions are revoked immediately; role change applies on next token issuance.

#### Business Rules
- **BR-MD-008** Staff invite requires unique identifier per tenant.
- **BR-MD-009** Deactivated staff cannot refresh or create new sessions.

### 4.4 Location Management
#### User Stories
**US-MD-009** Owner/Manager creates and edits locations.  
Acceptance Criteria: location slug unique within tenant; default location always exists; inactive locations cannot receive new operational writes.

**US-MD-010** Staff switches active location if assigned to multiple locations.  
Acceptance Criteria: switch endpoint validates assignment; context applied immediately to dashboard navigation and data.

#### Business Rules
- **BR-MD-010** Tenant must keep at least one active location.
- **BR-MD-011** Default location fallback applies to single-location staff sessions.

### 4.5 Product/Catalog Entry Points
#### User Stories
**US-MD-011** Staff with catalog permission accesses product, modifier, and tenant charge configuration modules through dashboard navigation.  
Acceptance Criteria: menu visibility is permission-aware; deep-links carry tenant/location context; unauthorized access blocked.

#### Business Rules
- **BR-MD-012** Dashboard is integration entry point; canonical CRUD rules remain in Product/Modifier/TenantCharge modules.

### 4.6 Orders/KDS Entry Points
#### User Stories
**US-MD-012** Operator accesses order queue and KDS overview as read-only in first iteration.  
Acceptance Criteria: list, status, and detail read views are available; mutation actions disabled unless explicitly enabled in later phase.

#### Business Rules
- **BR-MD-013** Iteration-1 order/KDS interactions are read-only except explicitly authorized workflows.

### 4.7 Reports Basics
#### User Stories
**US-MD-013** Owner/Manager sees daily sales summary by location.  
Acceptance Criteria: includes gross/net totals, transaction count, and average ticket.

**US-MD-014** Finance/Admin sees transaction ledger and refund summaries.  
Acceptance Criteria: report aligns with Transaction and OrderDetail ledger entities; export supports CSV.

#### Business Rules
- **BR-MD-014** Report totals must reconcile with immutable transaction ledger.
- **BR-MD-015** Refund views display original order linkage and actor attribution.

### 4.8 Notification Template Management (Optional)
#### User Stories
**US-MD-015** Owner/Manager configures message templates for operational notifications.  
Acceptance Criteria: templates can be listed, edited, activated/deactivated; changes are auditable.

#### Business Rules
- **BR-MD-016** Template activation requires channel compatibility validation.

## 5. Screen Reference List
- **SCR-MD-001** Staff OTP Login
- **SCR-MD-002** OTP Verification
- **SCR-MD-003** Session/Device Management
- **SCR-MD-004** Dashboard Home (role-aware)
- **SCR-MD-005** Tenant Settings
- **SCR-MD-006** Staff List & Invite
- **SCR-MD-007** Staff Detail (Role & Status)
- **SCR-MD-008** Location List & Switcher
- **SCR-MD-009** Location Detail/Edit
- **SCR-MD-010** Product/Catalog Entry Hub
- **SCR-MD-011** Orders & KDS Overview (Read-only v1)
- **SCR-MD-012** Daily Sales Summary
- **SCR-MD-013** Transaction Ledger & Refunds
- **SCR-MD-014** Audit Trail Viewer
- **SCR-MD-015** Notification Template Manager (Optional)

## 6. Data & Integration
### Required Reads/Writes and MVP-0 Dependencies
- **Identity/Auth dependency (PRD-01)**: Staff, OtpChallenge, StaffSession, StaffPermission.
- **Tenant/Location dependency (PRD-01)**: Tenant, Location, default-location provisioning.
- **Catalog dependency**: Product, ModifierGroup, ModifierChoice, ProductModifierGroup, TenantCharge.
- **Order dependency**: Order, OrderDetail, OrderItem, OrderItemModifier, OrderCharge, OrderRefund entities.
- **Ledger dependency**: Transaction for immutable financial reconciliation.
- **Notifications dependency**: NotificationTemplate and delivery channel integrations.

### Dashboard-Specific Entity Suggestions (if implemented)
- **AuditLog** (`AuditLogId uniqueidentifier`, `TenantId uniqueidentifier`, `LocationId uniqueidentifier NULL`, `ActorId uniqueidentifier`, `Action nvarchar(100)`, `TargetType nvarchar(100)`, `TargetId uniqueidentifier`, `Payload JSON`, `CreatedAt datetime2`).
- **StaffSession** (`StaffSessionId uniqueidentifier`, `StaffId uniqueidentifier`, `TenantId uniqueidentifier`, `LocationId uniqueidentifier NULL`, `RefreshTokenHash nvarchar(500)`, `IssuedAt datetime2`, `ExpiresAt datetime2`, `RevokedAt datetime2 NULL`, `DeviceInfo nvarchar(300) NULL`).
- **OtpChallenge** (`OtpChallengeId uniqueidentifier`, `TenantId uniqueidentifier`, `Principal nvarchar(150)`, `Channel nvarchar(50)`, `CodeHash nvarchar(500)`, `ExpiresAt datetime2`, `VerifiedAt datetime2 NULL`, `AttemptCount int`, `CreatedAt datetime2`).

## 7. Out of Scope (Document B)
1. Native mobile dashboard applications.
2. Advanced workflow automation/rule engine.
3. Full write-capable KDS and order orchestration redesign.
4. Marketplace/channel integrations beyond baseline adapters.
5. Advanced BI dashboards (cohort/LTV/forecasting).
6. Payroll and HRIS integrations.
7. Full accounting package synchronization.
8. Multi-tenant white-label theming marketplace.
9. Offline-first synchronization engine.
10. Custom report builder with arbitrary dimensions.

## 8. Dependencies
| Dependency Module | Dependency Type | Purpose in Merchant Dashboard |
|---|---|---|
| PRD-01 Platform Backbone | Hard | Staff auth, JWT claims, RBAC, tenant/location scope, shared APIs |
| Payments | Hard | Transaction and settlement data for reports and ledgers |
| Notifications | Soft/Optional | OTP delivery and operational template usage |
| Location | Hard | Location switch, scope enforcement, operational filtering |
| Charges (TenantCharge/OrderCharge) | Hard | Pricing/tax/service breakdown for reports and order context |
| Modifiers | Hard | Catalog entry points and order detail context |
| Orders/KDS | Hard (Read-only in v1) | Operational monitoring views and navigation entry points |
| Audit/Compliance | Hard | Sensitive-action traceability |

## 9. Traceability to Master Plan
Document B depends on MVP-0 backbone deliverables and feeds later milestone modules by enforcing shared identity, context scoping, and dashboard navigation standards.
