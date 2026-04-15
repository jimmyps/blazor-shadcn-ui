# PRD-01 Platform Backbone

## 1. Overview
Platform Backbone defines shared foundational capabilities for Neobiz OS, including tenant provisioning, identity, security, authorization, auditability, and common navigation context used by downstream product modules. This document is the prerequisite contract for PRD-09 Merchant Dashboard and other functional PRDs.

Cross-reference: PRD-00 Master Plan (Neobiz OS PRD suite), PRD-09 Merchant Dashboard.

## 2. Scope (MVP-0 Foundation)
- Tenant bootstrap and default location provisioning.
- Merchant/staff authentication using OTP and JWT.
- Session lifecycle with refresh-token rotation.
- RBAC authorization baseline for Owner/Manager/Staff personas.
- Tenant + location scoping for staff contexts.
- Shared entities/APIs needed by dashboard base navigation and security.

## 3. User Stories
### US-PB-001 Tenant Provisioning
**As** a new merchant owner, **I want** a tenant to be provisioned with a default location, **so that** dashboard onboarding is immediately usable.

**Acceptance Criteria**
1. `POST /api/tenants` creates `Tenant` and one default `Location` in one transaction.
2. Response includes `tenantId`, `defaultLocationId`, and bootstrap status.
3. If location creation fails, tenant creation is rolled back.

### US-PB-002 Staff OTP Sign-in
**As** merchant staff, **I want** OTP-based sign-in, **so that** passwordless login is secure and simple.

**Acceptance Criteria**
1. `POST /api/auth/staff/otp/request` issues an OTP challenge with rate limits.
2. `POST /api/auth/staff/otp/verify` returns access and refresh tokens on success.
3. Invalid or expired OTP attempts are logged in audit trail.

### US-PB-003 Scoped Staff Session
**As** a manager with multiple locations, **I want** to choose active location at login, **so that** actions are correctly scoped.

**Acceptance Criteria**
1. Staff token includes `tenantId`, `staffId`, `role`, and `locationId` claim.
2. For single-location tenant, `locationId` defaults to tenant default location.
3. API rejects write actions if requested `locationId` conflicts with token scope.

### US-PB-004 Session Rotation and Revocation
**As** platform security, **I want** rotating refresh tokens and revocation support, **so that** token replay risk is minimized.

**Acceptance Criteria**
1. Refresh token is single-use and rotated on each refresh request.
2. Prior refresh token is invalidated immediately after successful rotation.
3. Revoked sessions are denied for both access-token refresh and privileged operations.

### US-PB-005 Authorization Baseline
**As** owner/manager/staff, **I want** permission-based access control, **so that** features are limited by role.

**Acceptance Criteria**
1. Every dashboard endpoint maps to one or more named permissions.
2. Owner has full tenant-level permissions.
3. Manager and Staff have restricted permission sets configurable per tenant.

## 4. Business Rules
- **BR-PB-001** OTP requests are rate-limited per phone/email identifier, IP, and tenant.
- **BR-PB-002** OTP validity window is short-lived and one-time use.
- **BR-PB-003** Access tokens are short-lived JWTs; refresh tokens are opaque and revocable.
- **BR-PB-004** Refresh-token rotation is mandatory for every refresh exchange.
- **BR-PB-005** One `OwnerAccount` can own one or more `Tenant` records.
- **BR-PB-006** Tenant provisioning auto-creates a default `Location` and sets `Tenant.DefaultLocationId`.
- **BR-PB-007** Staff authorization is evaluated by permissions, not UI visibility alone.
- **BR-PB-008** Sensitive actions (role changes, token revocation, tenant/location changes) must be auditable.
- **BR-PB-009** Staff write operations require valid `tenantId` and `locationId` scope.
- **BR-PB-010** Location scope can be switched only to locations explicitly assigned to the staff actor.

## 5. Shared Entities and Type Conventions (SQL Server 2025)
Conventions:
- IDs: `uniqueidentifier`
- Monetary: `NUMERIC(19,4)`
- Date-time: `datetime2`
- JSON payloads: SQL Server 2025 native `JSON`
- Mutable entities include `CreatedAt`, `UpdatedAt`, and optionally `SyncedAt`

### Core Entities Required by Merchant Dashboard Auth + Base Navigation
- **Tenant**: business root and config owner.
- **Location**: physical or operational site under tenant.
- **Staff**: dashboard actor profile.
- **StaffPermission**: resolved permission mapping by role/overrides.
- **StaffSession**: active/revoked session records and refresh-token lineage.
- **OtpChallenge**: OTP request/verification tracking.
- **AuditLog**: immutable event ledger for sensitive actor actions.
- **NotificationTemplate**: optional shared dependency for OTP and operational templates.

## 6. Shared API Contracts (Backbone)
### Authentication
- `POST /api/auth/staff/otp/request`
- `POST /api/auth/staff/otp/verify`
- `POST /api/auth/staff/refresh`
- `POST /api/auth/staff/logout`

### Context and Navigation Prerequisites
- `GET /api/me` (profile, role, permissions, tenant context)
- `GET /api/me/locations` (assigned locations)
- `POST /api/me/location/switch` (active location switch)
- `GET /api/navigation` (menu tree resolved by permissions)

### Administration Primitives
- `GET /api/tenants/{tenantId}`
- `PATCH /api/tenants/{tenantId}`
- `GET /api/locations`
- `POST /api/locations`
- `PATCH /api/locations/{locationId}`

## 7. Entity Reference (MVP-0)
| Entity | Purpose | Notes |
|---|---|---|
| Tenant | Merchant business boundary | Holds business profile, defaults, and config flags |
| Location | Operational unit under tenant | Includes default location and active status |
| Staff | Dashboard user identity | Linked to role and assigned locations |
| StaffSession | Session + refresh-token rotation state | Supports revocation and audit correlation |
| OtpChallenge | OTP challenge lifecycle record | Used for throttling and fraud controls |
| StaffPermission | Effective permission grants | Derived from role + explicit overrides |
| AuditLog | Immutable event trail | Required for sensitive admin/auth actions |
| NotificationTemplate | Reusable OTP/operational templates | Shared with notification module |

## 8. Non-Functional Requirements
- Authentication endpoints p95 response target: <= 400ms under normal load.
- Session revocation propagation target: <= 30 seconds.
- Audit logs must be immutable and queryable by tenant, actor, event type, and time range.
- All protected endpoints require JWT validation, permission evaluation, and tenant/location scope enforcement.

## 9. Dependencies
- PRD-00 Master Plan (roadmap, release sequencing).
- Notifications module for OTP delivery channels.
- Merchant Dashboard (PRD-09) as primary consumer.
- Product/Order/Reporting modules for shared authorization and context propagation.
