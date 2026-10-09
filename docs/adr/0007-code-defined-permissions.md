# ADR 0007: Code-defined permissions

- **Status:** Accepted
- **Date:** 2026-10-09

## Context

Three staff roles (Owner, Secretary, Teacher) and six permissions exist today, and the full matrix between
them is known in advance — it does not vary per centre, per customer or at runtime. Something still has to
answer "does this role hold this permission" on every tenant-scoped request, and that answer has to be
reviewable, testable and impossible to get subtly wrong one row at a time.

## Alternatives considered

- **Database-defined roles and permissions** (tables an admin edits through a UI, each centre able to define
  its own roles or reassign permissions) is the usual shape once customers genuinely need to customize who
  can do what. It costs real weight today: a schema for roles and role-permission assignments, an admin UI
  and its own authorization (who may edit permissions?), migration or seeding for the default matrix, and an
  audit trail for changes to something security-relevant — all for a matrix that, right now, is the same for
  every centre and changes only when a new feature adds a new permission.
- **Code-defined permissions** (this decision) keeps the matrix as a static map, reviewed in a pull request
  like any other code change, covered by an exhaustive table test that states every cell literally rather
  than deriving it from the map under test. Changing what a role may do means a code change, a test change
  and a deploy — slower than an admin UI, but every change is attributable to a commit and a review, and
  there is no runtime path for a bug or a mistaken click to silently grant a permission.

## Decision

The permission catalogue (`Permissions`) and the role-permission map (`RolePermissions`) are fixed in code,
in the Domain layer, with no role or permission tables. `RolePermissionsTests` states all 18 role x
permission cells literally — expected values are never computed from the map it tests — so the exhaustive
table doubles as the audit: every cell is a decision someone can see in a diff. See
`docs/architecture/authorization.md` for the matrix and how it is enforced.

## Consequences

Adding a permission or changing what a role holds requires a code change, not an admin action — acceptable
while the matrix is small and shared across every centre, and arguably safer than making it editable before
there is a real need to. No centre can define its own roles or reassign permissions; if that becomes a real
requirement, migrating to database-defined roles later remains possible; it does not have to be decided now.

The `staff.view`/`staff.manage` rows this document already listed are live as of Day 30 (`GET`/`POST
/api/staff` and the role-change, deactivate and reactivate endpoints), proven by the 33-case permission
matrix (`PermissionMatrixTests`) over real HTTP, not just `RolePermissionsTests`' table. `audit.view` and
`centre.settings.manage` remain declared but unused until Day 32 — accepted ahead of time the same way the
staff rows were, not a reason to keep this decision in draft any longer.
