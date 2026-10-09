# Authorization

Who may do what, once tenancy (`docs/architecture/tenancy.md`) has already decided which centre a request
runs in. Written from the implementation; the matrix below is quoted from `RolePermissionsTests`
(`TutoringCentre.Domain.Tests`), not derived from the map it tests — if the two ever disagree, the test is
the source of truth and this file is wrong.

## The matrix

Three staff roles from Month 1 (Owner, Secretary, Teacher). The map is fixed in code
(`RolePermissions`, `TutoringCentre.Domain.Identity`) — no role or permission tables, no per-centre
overrides.

| Permission | Owner | Secretary | Teacher | Used by |
| --- | --- | --- | --- | --- |
| `subjects.view` | yes | yes | yes | list and get subjects |
| `subjects.manage` | yes | yes | no | create, rename, archive, restore subjects |
| `staff.view` | yes | no | no | list staff (Day 29) |
| `staff.manage` | yes | no | no | create staff, change role, deactivate, reactivate (Day 29) |
| `audit.view` | yes | no | no | read the audit log (Day 32) |
| `centre.settings.manage` | yes | no | no | read and update centre settings (Day 32) |

Permission keys are constants on `Permissions` (`TutoringCentre.Domain.Identity`), `<module>.<capability>`,
lower case, dot separated — never a free string. `RolePermissions.Holds(role, permission)` is the only place
that answers "does this role hold this permission"; an unknown role or an unknown permission key is denied,
never default-allowed, including for Owner — Owner holds every permission because the map lists all six
against it, not because of a bypass branch. Four rows (`staff.*`, `audit.view`, `centre.settings.manage`) are
defined today but unused until Days 29 and 32 — the catalogue and the map exist ahead of the use cases that
will declare them, so those use cases have nothing left to invent.

## The marker and the pipeline step

**The marker.** `IRequirePermission` (`TutoringCentre.Application.Common.Cqrs`), beside `ITenantScoped`,
exposes exactly one read-only `RequiredPermission`. One permission per request — no lists, no any/all
semantics — because a use case either needs a capability or it does not; splitting read from write (two
Subject permissions, not one) is what lets a role see data without being able to change it.

**The step.** `Dispatcher.CheckPermission` runs in both `SendAsync` and `QueryAsync`, immediately after the
tenant step (Layer 1, `docs/architecture/tenancy.md`) and before `IUnitOfWork.BeginAsync` — a denied request
never opens a transaction, the same rule the tenant step already follows. It is a no-op for a request that
does not implement `IRequirePermission`. For one that does:

- a system actor is always allowed, regardless of role;
- a staff actor is allowed only if `RolePermissions.Holds(actor.Role, permission)` is true;
- every other case — an anonymous actor, or a staff actor whose role does not hold the permission — is
  refused with `Error.Forbidden("auth.permission_denied", …)` (403), a generic message that never names the
  missing permission.

The step uses the role already on the actor (the session's `role` claim, revalidated on the cookie's own
schedule — `docs/architecture/authentication.md`); it does not load the membership from the database on
every request. On denial it logs one warning line — request type, permission, user id, centre id — never a
name or an email. Pipeline cases C15–C19 (`docs/notes/pipeline-cases.md`) specify and test the step directly;
C18/C19 prove the tenant step still runs first and wins when both would refuse.

**The architecture guarantee.** Three rules in `AuthorizationRuleTests`
(`TutoringCentre.Architecture.Tests`) keep this from rotting: every `ITenantScoped` request also implements
`IRequirePermission`, so a new tenant-scoped use case cannot exist without an explicit authorization
decision; every declared permission is a member of the `Permissions` catalogue, so a misspelt key fails the
build instead of silently denying everyone; and no Application command or query handler branches on a
`StaffRole` value directly (a source scan, with an allow-list reserved for Day 29's staff-management
handlers, which handle roles as data rather than as a decision) — authorization is something a request
declares, never something a handler decides for itself by comparing roles.

## What the UI does and does not do

`GET /api/me` returns `permissions`: the active role's keys, sorted, computed fresh from `RolePermissions` on
every call — empty when no centre is active. Nothing new in the cookie or the claims; the list is computed,
never stored, and a client is never asked to derive it from a role name. The frontend reads it through one
hook (`useCan`) and one wrapper (`Can`, both `frontend/src/features/session`), applied to the sidebar and the
Subjects page: a nav item or an action that needs a permission the session lacks is not rendered. A 403
`auth.permission_denied` from a mutation shows a translated toast and refetches `/api/me`, so a role changed
mid-session (by another admin, in another tab) is reflected without a full reload.

None of this is the security boundary. Hiding a control improves the experience; it protects nothing a
hand-made request couldn't already reach, had the server allowed it — which is exactly what the pipeline
step above does not. A `POST /api/subjects` with a valid body and a valid CSRF token, sent by a teacher
outside the UI entirely, still returns 403 and creates nothing (verified manually against the running API
during Task 28.9). The client adapts to permissions for usability; the server alone enforces them.

## Three different checks

Three questions, three places they're answered, one example each:

- **Tenancy — which centre?** "Is there a centre at all, and does this row belong to it?" Layer 1 (the
  dispatcher's tenant step) answers the first half; Layers 2–4 (`docs/architecture/tenancy.md`) answer the
  second. A teacher with no centre selected gets `tenant.not_selected` before permission is ever considered
  (pipeline cases C18/C19) — tenancy is checked first because nothing else means anything without it.
- **Permission — which action?** "Does this role hold this capability, in general, regardless of which row?"
  The step this document describes. A teacher's role never holds `subjects.manage`, so every write is
  `auth.permission_denied` no matter which subject, or whether it exists at all — Task 28.10's extra case (a
  forbidden rename against an unknown id is still 403, never 404) is the direct proof that permission is
  evaluated before existence.
- **Ownership — which record?** "Within an action this role may generally perform, is this the specific row
  it may act on?" Neither layer above answers this — tenancy stops at "this centre", permission stops at
  "this capability" — and nothing in the codebase answers it yet. A teacher assigned to only some of a
  centre's groups, who should see all subjects but only their own groups' attendance, is the Month 3 shape of
  this question; it is deferred, not solved by coincidence, because no Month 2 use case needs a check finer
  than centre and role together.

Confusing these is how access bugs start: a check written as "is this an owner" instead of "does this role
hold X" silently stops working the moment a fourth role exists; a check that stops at tenancy without also
checking permission lets any signed-in member of a centre do anything in it.
