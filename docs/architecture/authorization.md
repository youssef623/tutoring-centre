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
| `staff.view` | yes | no | no | `GET /api/staff` (Day 30) |
| `staff.manage` | yes | no | no | `POST /api/staff`, change role, deactivate, reactivate (Day 30) |
| `audit.view` | yes | no | no | read the audit log (Day 32) |
| `centre.settings.manage` | yes | no | no | read and update centre settings (Day 32) |
| `students.view` | yes | yes | no | list and get a student or parent, and the phone lookup (Days 39–41) |
| `students.manage` | yes | yes | no | register, update, archive, restore a student; create, update, link, unlink and set-primary a parent (Days 39–41) |
| `teachers.view` | yes | yes | no | list and get teachers (Day 43) |
| `teachers.manage` | yes | no | no | create, update, link an account to, archive, restore a teacher (Day 43) |
| `groups.view` | yes | yes | yes | list and get groups — a teacher only their own, see "Ownership" below (Days 44–45) |
| `groups.manage` | yes | yes | no | create, rename, change capacity, assign a teacher, archive, restore a group (Days 44–45) |

Permission keys are constants on `Permissions` (`TutoringCentre.Domain.Identity`), `<module>.<capability>`,
lower case, dot separated — never a free string. `RolePermissions.Holds(role, permission)` is the only place
that answers "does this role hold this permission"; an unknown role or an unknown permission key is denied,
never default-allowed, including for Owner — Owner holds every permission because the map lists all twelve
against it, not because of a bypass branch. The `students.*` pair splits exactly like the others: the parent
phone lookup used while registering a student (`POST /api/parents/lookup`) only needs `students.view` — it
changes nothing — even though it is reachable only from a `students.manage`-gated screen today.

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
hook (`useCan`) and one wrapper (`Can`, both `frontend/src/features/session`), applied to the sidebar and
every permission-gated action (the Subjects page's write controls; the Staff page's Add button and row
menus, Day 33): a nav item or an action that needs a permission the session lacks is not rendered. A 403
`auth.permission_denied` from a mutation shows a translated toast and refetches `/api/me`, so a role changed
mid-session (by another admin, in another tab) is reflected without a full reload.

A whole page can need gating too, for a user who types the URL directly rather than following a hidden nav
link: `RouteGuard` (Day 33, same module) wraps the Staff, Settings and Audit log routes, each tied to its
`staff.view` / `centre.settings.manage` / `audit.view` permission — and, since Day 41–42, the Students list and
detail pages (`students.view`) and the registration page (`students.manage`), and since Day 46 the Teachers page
(`teachers.view`) and the Groups list and group pages (`groups.view`); a student's parents card then
hides its own manage affordances (link, unlink, make primary, add a fifth parent) the same way `Can` does
everywhere else, never as the actual enforcement. Unlike `Can`, which renders nothing, a
denied `RouteGuard` renders a translated "you don't have access" state with a link back to the dashboard —
and critically, the page's own data hooks live inside its children, so they never mount and no request for
the guarded data is ever issued. Still not the security boundary: a hand-made request to `GET /api/staff` as
a teacher returns 403 from the pipeline step regardless of what the route rendered.

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
  "this capability". Month 2 needed nothing finer than centre and role together; since Days 44–45 groups
  answer it for teachers: `groups.view` lets a teacher read groups, and the teacher scope (next section)
  decides which ones.

## Ownership: the teacher scope

A Teacher holds `groups.view` but must see only the groups it teaches. That is a scope, not a permission
key — there is no `groups.view_own` — and it is the third check on a request, after tenancy and permission.

- **The rule lives in the Domain.** `RoleScopes.IsLimitedToOwnGroups(role)` (`TutoringCentre.Domain.Identity`,
  beside `RolePermissions`) is true for Teacher only. Handlers never compare `StaffRole` values themselves
  (`AuthorizationRuleTests` rule 3); they see only the resolved value below.
- **The value.** `TeacherScope` (`TutoringCentre.Application.Common.Security`) is one of `All`, `None` or
  `Own(teacherId)`, with `Allows(teacherId)`. `ITeacherScopeProvider.GetAsync` resolves it once per request:
  a system actor (the seed CLI, background work) gets `All`; a staff role not limited to its own groups gets
  `All`; a limited role gets `Own` of the active teacher record linked to its user in the acting centre
  (`ITeacherReadService.FindIdByUserAsync`), or `None` when there is no linked record or it is archived; any
  other actor gets `None`. It fails closed: an unlinked teacher sees nothing, never everything.
- **Where it is applied.** Every `IGroupReadService` method takes a `TeacherScope` as a required parameter —
  there is no overload without one, so a new read cannot forget it — and the predicate is part of the SQL, so
  out-of-scope rows never leave the database. `ListGroupsQuery` and `GetGroupQuery` resolve the scope and
  pass it down. The endpoints (`GroupEndpoints`) hold no role or scope logic and there is no separate "my
  groups" route: one route, different results per actor, nothing to bypass by calling the "other" route.
  Filters intersect with the scope and never widen it (`?teacherId=<someone else>` returns an empty list).
- **Out of scope is 404, not 403.** A teacher reading another teacher's group gets `group.not_found`, exactly
  as for an unknown ID or another centre's group, so the response never confirms that the group exists. A
  403 would be a disclosure. Writes are different: a teacher's write to its own group is still 403
  `auth.permission_denied`, because permission is evaluated before ownership and a Teacher does not hold
  `groups.manage`.
- **Tenancy and ownership compose.** The provider looks the teacher record up in the acting centre, so one
  account linked to a teacher record in each of two centres sees a different set of groups per selected
  centre, and never another centre's group in either.
- **What the screens do (Day 46).** The Groups pages never ask "is this a teacher": they render whatever the
  scoped API returns, and branch only on permissions — `Can` with `groups.manage` around every write, the
  teacher filter only with `teachers.view` (so a Teacher's session never calls `/api/teachers` at all), and a
  distinct empty-state text chosen by the absence of `groups.manage`. A group page answered with 404 renders
  one "Group not found" state for an unknown ID, another centre's group and someone else's group alike, so the
  UI adds nothing the 404 itself withholds. None of this is enforcement: removing a `Can` exposes a button whose
  request the API still refuses.
- **The scope follows the current role.** It is resolved per request from the session's role and the linked
  teacher record, never stored in the session: once a role change has gone through the session's
  revalidation (`docs/architecture/authentication.md`), a former Teacher now holding Secretary resolves to
  `All`; removing the account link makes the same Teacher's next list empty (`GroupTeacherScopeTests`).
- **Tests answer different questions.** `PermissionMatrixTests` records the permission decision only: Teacher
  is "allowed" on the two group reads (on a group it owns) and 403 on the six writes. What a teacher sees is
  proven separately, through real sessions, by `GroupTeacherScopeTests` (linked, unlinked, reassigned,
  unlinked again, and the other centre), and in the Infrastructure layer by `TeacherScopeProviderTests` and
  `GroupTeacherScopeTests` against PostgreSQL.

## Accepted disclosure: CreateStaffCommand reveals whether an email already has an account

`CreateStaffCommand` (Day 29) ensures a login account for the email it is given, then returns whether that
account was newly created or already existed (no temporary password in the latter case). An owner who tries
to add someone already registered elsewhere therefore learns, indirectly, that the email has a Hessa account
somewhere — not which centre, not under which name. This is accepted, not overlooked: the caller must already
be an authenticated owner with `staff.manage` in a real centre (nothing here is reachable anonymously or by a
lower role), and every call goes through the dispatcher's existing structured outcome log like any other
command. The alternative — always claiming to create a new account, then silently discarding a second
password — would mean an owner occasionally sees no temporary password for a person who is, in fact, brand
new, with no way to tell that apart from a typo; this trade was judged worse than the narrow disclosure above.

Confusing these is how access bugs start: a check written as "is this an owner" instead of "does this role
hold X" silently stops working the moment a fourth role exists; a check that stops at tenancy without also
checking permission lets any signed-in member of a centre do anything in it.

## The first-login gate is not a fourth check

A pending password change (`ApplicationUser.MustChangePassword`, Day 30) is not a permission, a tenancy
question or an ownership question — it is a precondition on *having* a centre at all. It is enforced where
a centre ID enters a session in the first place: login never auto-selects one while the flag is set, and
`GetActiveMembershipQuery` (`docs/architecture/authentication.md`) refuses centre selection outright with
`auth.password_change_required` before it even looks at membership. Every permission check downstream is
therefore moot for a gated user — there is no centre, so every tenant-scoped request already fails with
`tenant.not_selected` on its own. No pipeline step, no new error kind, no change to `RolePermissions`.
